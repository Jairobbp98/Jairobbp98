import { Test, TestingModule } from '@nestjs/testing';
import { getRepositoryToken } from '@nestjs/typeorm';
import { ConflictException, NotFoundException } from '@nestjs/common';
import { CompaniesService } from './companies.service';
import { Company } from './entities/company.entity';

const mockCompany: Partial<Company> = {
  id: '123e4567-e89b-12d3-a456-426614174001',
  name: 'Test Company',
  description: 'A test company',
  industry: 'Technology',
  isActive: true,
  createdAt: new Date(),
  updatedAt: new Date(),
};

const mockRepository = {
  create: jest.fn(),
  save: jest.fn(),
  findOne: jest.fn(),
  findAndCount: jest.fn(),
  remove: jest.fn(),
};

describe('CompaniesService', () => {
  let service: CompaniesService;

  beforeEach(async () => {
    const module: TestingModule = await Test.createTestingModule({
      providers: [
        CompaniesService,
        {
          provide: getRepositoryToken(Company),
          useValue: mockRepository,
        },
      ],
    }).compile();

    service = module.get<CompaniesService>(CompaniesService);
    jest.clearAllMocks();
  });

  it('should be defined', () => {
    expect(service).toBeDefined();
  });

  describe('create', () => {
    it('should create a company successfully', async () => {
      mockRepository.findOne.mockResolvedValue(null);
      mockRepository.create.mockReturnValue(mockCompany);
      mockRepository.save.mockResolvedValue(mockCompany);

      const result = await service.create({ name: 'Test Company' });

      expect(result).toEqual(mockCompany);
      expect(mockRepository.save).toHaveBeenCalled();
    });

    it('should throw ConflictException if company name exists', async () => {
      mockRepository.findOne.mockResolvedValue(mockCompany);

      await expect(service.create({ name: 'Test Company' })).rejects.toThrow(
        ConflictException,
      );
    });
  });

  describe('findAll', () => {
    it('should return paginated companies', async () => {
      mockRepository.findAndCount.mockResolvedValue([[mockCompany], 1]);

      const result = await service.findAll({ page: 1, limit: 10 });

      expect(result.data).toHaveLength(1);
      expect(result.total).toBe(1);
    });
  });

  describe('findOne', () => {
    it('should return a company by ID', async () => {
      mockRepository.findOne.mockResolvedValue(mockCompany);

      const result = await service.findOne(mockCompany.id);
      expect(result).toEqual(mockCompany);
    });

    it('should throw NotFoundException if company not found', async () => {
      mockRepository.findOne.mockResolvedValue(null);

      await expect(service.findOne('nonexistent-id')).rejects.toThrow(
        NotFoundException,
      );
    });
  });
});
