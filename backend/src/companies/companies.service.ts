import {
  Injectable,
  NotFoundException,
  ConflictException,
} from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Company } from './entities/company.entity';
import { CreateCompanyDto } from './dto/create-company.dto';
import { UpdateCompanyDto } from './dto/update-company.dto';

@Injectable()
export class CompaniesService {
  constructor(
    @InjectRepository(Company)
    private readonly companiesRepository: Repository<Company>,
  ) {}

  async create(createCompanyDto: CreateCompanyDto): Promise<Company> {
    const existing = await this.companiesRepository.findOne({
      where: { name: createCompanyDto.name },
    });
    if (existing) {
      throw new ConflictException('Company with this name already exists');
    }

    const company = this.companiesRepository.create(createCompanyDto);
    return this.companiesRepository.save(company);
  }

  async findAll(options?: {
    page?: number;
    limit?: number;
    industry?: string;
    isActive?: boolean;
  }): Promise<{ data: Company[]; total: number; page: number; limit: number }> {
    const { page = 1, limit = 10, industry, isActive } = options || {};

    const where: any = {};
    if (industry) where.industry = industry;
    if (isActive !== undefined) where.isActive = isActive;

    const [data, total] = await this.companiesRepository.findAndCount({
      where,
      skip: (page - 1) * limit,
      take: limit,
      order: { name: 'ASC' },
    });

    return { data, total, page, limit };
  }

  async findOne(id: string): Promise<Company> {
    const company = await this.companiesRepository.findOne({
      where: { id },
      relations: ['users', 'documents'],
    });
    if (!company) {
      throw new NotFoundException(`Company with ID ${id} not found`);
    }
    return company;
  }

  async update(id: string, updateCompanyDto: UpdateCompanyDto): Promise<Company> {
    const company = await this.findOne(id);

    if (updateCompanyDto.name && updateCompanyDto.name !== company.name) {
      const existing = await this.companiesRepository.findOne({
        where: { name: updateCompanyDto.name },
      });
      if (existing) {
        throw new ConflictException('Company name already in use');
      }
    }

    Object.assign(company, updateCompanyDto);
    return this.companiesRepository.save(company);
  }

  async remove(id: string): Promise<void> {
    const company = await this.findOne(id);
    await this.companiesRepository.remove(company);
  }
}
