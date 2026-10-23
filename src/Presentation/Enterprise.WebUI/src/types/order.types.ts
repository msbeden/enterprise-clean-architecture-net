import type { MoneyDto } from './product.types';

export interface OrderItemDto {
  productId: string;
  productName: string;
  unitPrice: MoneyDto;
  quantity: number;
  totalPrice: MoneyDto;
}

export interface OrderDto {
  id: string;
  userId: string;
  customerName?: string;
  orderDate: string;
  status: string;
  totalAmount: MoneyDto;
  items: OrderItemDto[];
  shippingAddress: string;
}

export interface CreateOrderItemDto {
  productId: string;
  quantity: number;
  unitPriceAmount: number;
  currency: string;
}

export interface CreateOrderCommand {
  items: CreateOrderItemDto[];
  shippingAddress: string;
}
