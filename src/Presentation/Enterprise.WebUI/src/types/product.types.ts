export interface MoneyDto {
  amount: number;
  currency: string;
}

export interface ProductDto {
  id: string;
  name: string;
  description: string;
  sku: string;
  price: MoneyDto;
  stockQuantity: number;
  categoryId: string;
  categoryName?: string;
  imageUrl?: string;
}

export interface CreateProductCommand {
  name: string;
  description: string;
  sku: string;
  priceAmount: number;
  currency: string;
  stockQuantity: number;
  categoryId: string;
  imageUrl?: string;
}

export interface GetProductsQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  categoryId?: string;
}

export interface UpdateProductCommand extends CreateProductCommand {
  id: string;
}

