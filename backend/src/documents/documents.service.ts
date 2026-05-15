import {
  Injectable,
  NotFoundException,
  ForbiddenException,
} from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import * as PDFDocument from 'pdfkit';
import { Document, DocumentType, DocumentStatus } from './entities/document.entity';
import { CreateDocumentDto } from './dto/create-document.dto';
import { UpdateDocumentDto } from './dto/update-document.dto';
import { UserRole } from '../users/enums/user-role.enum';
import { User } from '../users/entities/user.entity';

const UNKNOWN_AUTHOR = 'Unknown';
const APP_NAME = 'Management System';
const DATE_LOCALE = 'en-US';

@Injectable()
export class DocumentsService {
  constructor(
    @InjectRepository(Document)
    private readonly documentsRepository: Repository<Document>,
  ) {}

  async create(createDocumentDto: CreateDocumentDto, author: User): Promise<Document> {
    const document = this.documentsRepository.create({
      ...createDocumentDto,
      authorId: author.id,
      type: createDocumentDto.type || DocumentType.MARKDOWN,
    });
    return this.documentsRepository.save(document);
  }

  async findAll(options?: {
    page?: number;
    limit?: number;
    authorId?: string;
    companyId?: string;
    type?: DocumentType;
    status?: DocumentStatus;
  }): Promise<{ data: Document[]; total: number; page: number; limit: number }> {
    const { page = 1, limit = 10, ...filters } = options || {};

    const where: any = {};
    if (filters.authorId) where.authorId = filters.authorId;
    if (filters.companyId) where.companyId = filters.companyId;
    if (filters.type) where.type = filters.type;
    if (filters.status) where.status = filters.status;

    const [data, total] = await this.documentsRepository.findAndCount({
      where,
      skip: (page - 1) * limit,
      take: limit,
      relations: ['author', 'company'],
      order: { updatedAt: 'DESC' },
    });

    return { data, total, page, limit };
  }

  async findOne(id: string): Promise<Document> {
    const document = await this.documentsRepository.findOne({
      where: { id },
      relations: ['author', 'company'],
    });
    if (!document) {
      throw new NotFoundException(`Document with ID ${id} not found`);
    }
    return document;
  }

  async update(
    id: string,
    updateDocumentDto: UpdateDocumentDto,
    user: User,
  ): Promise<Document> {
    const document = await this.findOne(id);

    if (document.authorId !== user.id && user.role !== UserRole.ADMIN) {
      throw new ForbiddenException('You can only update your own documents');
    }

    Object.assign(document, updateDocumentDto);
    return this.documentsRepository.save(document);
  }

  async remove(id: string, user: User): Promise<void> {
    const document = await this.findOne(id);

    if (document.authorId !== user.id && user.role !== UserRole.ADMIN) {
      throw new ForbiddenException('You can only delete your own documents');
    }

    await this.documentsRepository.remove(document);
  }

  async publish(id: string, user: User): Promise<Document> {
    return this.update(id, { status: DocumentStatus.PUBLISHED }, user);
  }

  async archive(id: string, user: User): Promise<Document> {
    return this.update(id, { status: DocumentStatus.ARCHIVED }, user);
  }

  async generatePdf(id: string): Promise<Buffer> {
    const document = await this.findOne(id);

    return new Promise((resolve, reject) => {
      const doc = new PDFDocument({
        size: 'A4',
        margins: { top: 72, bottom: 72, left: 72, right: 72 },
      });

      const buffers: Buffer[] = [];
      doc.on('data', (chunk) => buffers.push(chunk));
      doc.on('end', () => resolve(Buffer.concat(buffers)));
      doc.on('error', reject);

      // Header
      doc
        .font('Helvetica-Bold')
        .fontSize(24)
        .fillColor('#2d2d2d')
        .text(document.title, { align: 'center' });

      doc.moveDown();

      // Metadata
      doc
        .font('Helvetica')
        .fontSize(10)
        .fillColor('#666666')
        .text(`Author: ${document.author?.fullName || UNKNOWN_AUTHOR}`)
        .text(`Created: ${document.createdAt?.toLocaleDateString(DATE_LOCALE)}`)
        .text(`Status: ${document.status}`)
        .text(`Type: ${document.type}`);

      if (document.tags) {
        doc.text(`Tags: ${document.tags}`);
      }

      doc.moveDown();

      // Divider
      doc
        .moveTo(72, doc.y)
        .lineTo(doc.page.width - 72, doc.y)
        .strokeColor('#e0e0e0')
        .stroke();

      doc.moveDown();

      // Content
      if (document.content) {
        const plainContent = document.content
          .replace(/#{1,6}\s/g, '')
          .replace(/\*\*(.*?)\*\*/g, '$1')
          .replace(/\*(.*?)\*/g, '$1')
          .replace(/`(.*?)`/g, '$1')
          .replace(/\[([^\]]+)\]\([^)]+\)/g, '$1');

        doc
          .font('Helvetica')
          .fontSize(12)
          .fillColor('#2d2d2d')
          .text(plainContent, { align: 'left', lineGap: 4 });
      }

      // Footer
      doc
        .font('Helvetica')
        .fontSize(9)
        .fillColor('#999999')
        .text(
          `Generated by ${APP_NAME} | ${new Date().toISOString()}`,
          72,
          doc.page.height - 50,
          { align: 'center' },
        );

      doc.end();
    });
  }

  async generateMarkdown(id: string): Promise<string> {
    const document = await this.findOne(id);

    const frontmatter = [
      '---',
      `title: "${document.title}"`,
      `author: "${document.author?.fullName || UNKNOWN_AUTHOR}"`,
      `date: "${document.createdAt?.toISOString()}"`,
      `status: "${document.status}"`,
      `type: "${document.type}"`,
      document.tags ? `tags: [${document.tags.split(',').map((t) => `"${t.trim()}"`).join(', ')}]` : '',
      '---',
      '',
    ]
      .filter(Boolean)
      .join('\n');

    return `${frontmatter}\n# ${document.title}\n\n${document.content || ''}`;
  }
}
