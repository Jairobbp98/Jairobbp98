import {
  IsEnum,
  IsNotEmpty,
  IsOptional,
  IsString,
  IsUUID,
} from 'class-validator';
import { ApiProperty, ApiPropertyOptional } from '@nestjs/swagger';
import { DocumentType } from '../entities/document.entity';

export class CreateDocumentDto {
  @ApiProperty({ example: 'Q3 Financial Report' })
  @IsString()
  @IsNotEmpty()
  title: string;

  @ApiPropertyOptional({ example: '# Report\n\nThis is the content...' })
  @IsString()
  @IsOptional()
  content?: string;

  @ApiPropertyOptional({ enum: DocumentType, default: DocumentType.MARKDOWN })
  @IsEnum(DocumentType)
  @IsOptional()
  type?: DocumentType;

  @ApiPropertyOptional({ example: 'finance,report,q3' })
  @IsString()
  @IsOptional()
  tags?: string;

  @ApiPropertyOptional({ example: 'company-uuid' })
  @IsUUID()
  @IsOptional()
  companyId?: string;
}
