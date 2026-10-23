export interface CategoryDto {
  id: string;
  name: string;
  description: string;
  productCount?: number;
}

export interface CreateCategoryCommand {
  name: string;
  description: string;
}

export interface UpdateCategoryCommand extends CreateCategoryCommand {
  id: string;
}

