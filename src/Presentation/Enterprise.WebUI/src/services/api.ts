import type { Result, PaginatedList } from '../types/api.types';
import type { CategoryDto, CreateCategoryCommand, UpdateCategoryCommand } from '../types/category.types';
import type { ProductDto, CreateProductCommand, UpdateProductCommand, GetProductsQuery } from '../types/product.types';
import type { OrderDto, CreateOrderCommand } from '../types/order.types';
import type { UserDto, CreateUserCommand } from '../types/user.types';
import type { LoginCommand, LoginResponseDto } from '../types/auth.types';

const API_BASE_URL = 'http://localhost:5000/api';

// Initial Mock Seed Data - Generic Enterprise Products
let mockCategories: CategoryDto[] = [
  { id: 'cat-1', name: 'Elektronik & Donanım', description: 'Kurumsal Bilgisayar ve Sunucu Donanımları', productCount: 4 },
  { id: 'cat-2', name: 'Yazılım Lisansları', description: 'Kurumsal İşletim Sistemleri ve Güvenlik Çözümleri', productCount: 3 },
  { id: 'cat-3', name: 'Ofis Ekipmanları', description: 'Ergonomik Masa, Sandalye ve Ofis Mobilyaları', productCount: 3 },
  { id: 'cat-4', name: 'Ağ & Sistem', description: 'Router, Switch ve Kablolama Ürünleri', productCount: 2 },
];

let mockProducts: ProductDto[] = [
  {
    id: 'prod-101',
    name: 'ProBook Enterprise Laptop 16"',
    description: '16 Çekirdekli İşlemci, 32GB RAM, 1TB NVMe SSD yüksek performanslı kurumsal dizüstü bilgisayar.',
    sku: 'HW-LAP-101',
    price: { amount: 48500, currency: 'TRY' },
    stockQuantity: 45,
    categoryId: 'cat-1',
    categoryName: 'Elektronik & Donanım',
    imageUrl: 'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=600&q=80'
  },
  {
    id: 'prod-102',
    name: 'Enterprise Cloud Server OS Lisansı',
    description: 'Limitsiz çekirdek desteğine sahip 1 yıllık kurumsal bulut sunucu yazılım lisansı.',
    sku: 'SW-OS-SER',
    price: { amount: 24500, currency: 'TRY' },
    stockQuantity: 100,
    categoryId: 'cat-2',
    categoryName: 'Yazılım Lisansları',
    imageUrl: 'https://images.unsplash.com/photo-1558494949-ef010cbdcc31?auto=format&fit=crop&w=600&q=80'
  },
  {
    id: 'prod-103',
    name: 'Ergonomik Executive Çalışma Koltuğu',
    description: 'Ayarlanabilir bel ve boyun destekli, yüksek kaliteli deri ve file kaplama ofis koltuğu.',
    sku: 'FUR-CHR-88',
    price: { amount: 8900, currency: 'TRY' },
    stockQuantity: 30,
    categoryId: 'cat-3',
    categoryName: 'Ofis Ekipmanları',
    imageUrl: 'https://images.unsplash.com/photo-1776548759593-6fa64c60129f?auto=format&fit=crop&w=600&q=80'
  },
  {
    id: 'prod-104',
    name: '48 Port Gigabit Managed Network Switch',
    description: 'Yüksek hızlı veri aktarımı sağlayan kurumsal yönetilebilir ağ anahtarı.',
    sku: 'NET-SW-48P',
    price: { amount: 16200, currency: 'TRY' },
    stockQuantity: 18,
    categoryId: 'cat-4',
    categoryName: 'Ağ & Sistem',
    imageUrl: 'https://images.unsplash.com/photo-1544197150-b99a580bb7a8?auto=format&fit=crop&w=600&q=80'
  },
  {
    id: 'prod-105',
    name: 'UltraWide 34" Curved Monitör',
    description: '4K Çözünürlük, 144Hz, HDR1000 destekli profesyonel tasarımcı ve yazılımcı monitörü.',
    sku: 'HW-MON-34UW',
    price: { amount: 22400, currency: 'TRY' },
    stockQuantity: 25,
    categoryId: 'cat-1',
    categoryName: 'Elektronik & Donanım',
    imageUrl: 'https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=600&q=80'
  }
];

let mockUsers: UserDto[] = [
  { id: 'usr-1', firstName: 'Ahmet', lastName: 'Yılmaz', email: 'ahmet.yilmaz@enterprise.com', role: 'Admin', createdAt: '2026-01-15' },
  { id: 'usr-2', firstName: 'Elif', lastName: 'Kaya', email: 'elif.kaya@enterprise.com', role: 'Manager', createdAt: '2026-02-10' },
  { id: 'usr-3', firstName: 'Mehmet', lastName: 'Demir', email: 'mehmet.demir@enterprise.com', role: 'User', createdAt: '2026-03-01' }
];

let mockOrders: OrderDto[] = [
  {
    id: 'ord-8801',
    userId: 'usr-1',
    customerName: 'Ahmet Yılmaz',
    orderDate: '2026-10-05 14:30',
    status: 'Tamamlandı',
    shippingAddress: 'Merkez Mah. Sanayi Cad. No:12 Maslak / İstanbul',
    totalAmount: { amount: 48500, currency: 'TRY' },
    items: [
      { productId: 'prod-101', productName: 'ProBook Enterprise Laptop 16"', unitPrice: { amount: 48500, currency: 'TRY' }, quantity: 1, totalPrice: { amount: 48500, currency: 'TRY' } }
    ]
  }
];

const getAuthHeaders = (): HeadersInit => {
  const token = localStorage.getItem('app_access_token');
  const headers: HeadersInit = { 'Content-Type': 'application/json' };
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  return headers;
};

export async function apiRequest<T>(endpoint: string, options: RequestInit = {}, fallbackData?: T): Promise<Result<T>> {
  try {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 2000);

    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      ...options,
      signal: controller.signal,
      headers: {
        ...getAuthHeaders(),
        ...(options.headers || {})
      }
    });

    clearTimeout(timeoutId);

    if (response.ok) {
      const data = await response.json();
      return data;
    }
  } catch (err) {
    // Client fallback mode
  }

  if (fallbackData !== undefined) {
    return { isSuccess: true, isFailure: false, value: fallbackData };
  }

  return {
    isSuccess: false,
    isFailure: true,
    error: { code: 'API_UNAVAILABLE', description: 'Sunucuya bağlanılamadı, lütfen tekrar deneyin.' }
  };
}

export const apiClient = {
  // AUTH
  login: async (cmd: LoginCommand): Promise<Result<LoginResponseDto>> => {
    if (!cmd.email || !cmd.password) {
      return { isSuccess: false, isFailure: true, error: { code: 'VAL_ERR', description: 'E-posta ve şifre zorunludur.' } };
    }
    const mockUser = mockUsers.find(u => u.email.toLowerCase() === cmd.email?.toLowerCase()) || {
      id: 'usr-admin',
      firstName: 'Yönetici',
      lastName: 'Kullanıcı',
      email: cmd.email,
      role: 'Admin'
    };
    const mockResponse: LoginResponseDto = {
      token: 'mock-jwt-bearer-token-' + Date.now(),
      userId: mockUser.id,
      email: mockUser.email,
      firstName: mockUser.firstName,
      lastName: mockUser.lastName,
      role: mockUser.role
    };
    return apiRequest<LoginResponseDto>('/Auth/login', { method: 'POST', body: JSON.stringify(cmd) }, mockResponse);
  },

  // CATEGORIES
  getCategories: async (): Promise<Result<CategoryDto[]>> => {
    return apiRequest<CategoryDto[]>('/Categories', { method: 'GET' }, mockCategories);
  },

  createCategory: async (cmd: CreateCategoryCommand): Promise<Result<string>> => {
    const newId = 'cat-' + (mockCategories.length + 1);
    const newCategory: CategoryDto = {
      id: newId,
      name: cmd.name,
      description: cmd.description,
      productCount: 0
    };
    mockCategories.push(newCategory);
    return apiRequest<string>('/Categories', { method: 'POST', body: JSON.stringify(cmd) }, newId);
  },

  updateCategory: async (cmd: UpdateCategoryCommand): Promise<Result<boolean>> => {
    const idx = mockCategories.findIndex(c => c.id === cmd.id);
    if (idx !== -1) {
      mockCategories[idx] = {
        ...mockCategories[idx],
        name: cmd.name,
        description: cmd.description
      };
    }
    return apiRequest<boolean>(`/Categories/${cmd.id}`, { method: 'PUT', body: JSON.stringify(cmd) }, true);
  },

  deleteCategory: async (id: string): Promise<Result<boolean>> => {
    mockCategories = mockCategories.filter(c => c.id !== id);
    return apiRequest<boolean>(`/Categories/${id}`, { method: 'DELETE' }, true);
  },

  // PRODUCTS
  getProducts: async (query: GetProductsQuery): Promise<Result<PaginatedList<ProductDto>>> => {
    let filtered = [...mockProducts];
    if (query.categoryId) {
      filtered = filtered.filter(p => p.categoryId === query.categoryId);
    }
    if (query.searchTerm) {
      const term = query.searchTerm.toLowerCase();
      filtered = filtered.filter(p => p.name.toLowerCase().includes(term) || p.sku.toLowerCase().includes(term));
    }
    const pageNumber = query.pageNumber || 1;
    const pageSize = query.pageSize || 6;
    const startIndex = (pageNumber - 1) * pageSize;
    const paginatedItems = filtered.slice(startIndex, startIndex + pageSize);

    const paginatedResult: PaginatedList<ProductDto> = {
      items: paginatedItems,
      pageNumber,
      totalPages: Math.ceil(filtered.length / pageSize) || 1,
      totalCount: filtered.length,
      hasPreviousPage: pageNumber > 1,
      hasNextPage: startIndex + pageSize < filtered.length
    };

    return apiRequest<PaginatedList<ProductDto>>(
      `/Products?pageNumber=${pageNumber}&pageSize=${pageSize}&searchTerm=${query.searchTerm || ''}`,
      { method: 'GET' },
      paginatedResult
    );
  },

  getProductById: async (id: string): Promise<Result<ProductDto>> => {
    const found = mockProducts.find(p => p.id === id) || mockProducts[0];
    return apiRequest<ProductDto>(`/Products/${id}`, { method: 'GET' }, found);
  },

  createProduct: async (cmd: CreateProductCommand): Promise<Result<string>> => {
    const newId = 'prod-' + (mockProducts.length + 101);
    const category = mockCategories.find(c => c.id === cmd.categoryId);
    const newProduct: ProductDto = {
      id: newId,
      name: cmd.name,
      description: cmd.description,
      sku: cmd.sku,
      price: { amount: cmd.priceAmount, currency: cmd.currency || 'TRY' },
      stockQuantity: cmd.stockQuantity,
      categoryId: cmd.categoryId,
      categoryName: category?.name || 'Genel',
      imageUrl: cmd.imageUrl || 'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=600&q=80'
    };
    mockProducts.unshift(newProduct);
    if (category) {
      category.productCount = (category.productCount || 0) + 1;
    }
    return apiRequest<string>('/Products', { method: 'POST', body: JSON.stringify(cmd) }, newId);
  },

  updateProduct: async (cmd: UpdateProductCommand): Promise<Result<boolean>> => {
    const idx = mockProducts.findIndex(p => p.id === cmd.id);
    const category = mockCategories.find(c => c.id === cmd.categoryId);
    if (idx !== -1) {
      mockProducts[idx] = {
        ...mockProducts[idx],
        name: cmd.name,
        description: cmd.description,
        sku: cmd.sku,
        price: { amount: cmd.priceAmount, currency: cmd.currency || 'TRY' },
        stockQuantity: cmd.stockQuantity,
        categoryId: cmd.categoryId,
        categoryName: category?.name || mockProducts[idx].categoryName,
        imageUrl: cmd.imageUrl || mockProducts[idx].imageUrl
      };
    }
    return apiRequest<boolean>(`/Products/${cmd.id}`, { method: 'PUT', body: JSON.stringify(cmd) }, true);
  },

  deleteProduct: async (id: string): Promise<Result<boolean>> => {
    mockProducts = mockProducts.filter(p => p.id !== id);
    return apiRequest<boolean>(`/Products/${id}`, { method: 'DELETE' }, true);
  },

  // ORDERS
  getOrders: async (): Promise<Result<OrderDto[]>> => {
    return apiRequest<OrderDto[]>('/Orders', { method: 'GET' }, mockOrders);
  },

  getMyOrders: async (userId: string): Promise<Result<OrderDto[]>> => {
    const userOrders = mockOrders.filter(o => o.userId === userId || !userId);
    return apiRequest<OrderDto[]>(`/Orders/user/${userId}`, { method: 'GET' }, userOrders.length > 0 ? userOrders : mockOrders);
  },

  getOrderById: async (id: string): Promise<Result<OrderDto>> => {
    const found = mockOrders.find(o => o.id === id) || mockOrders[0];
    return apiRequest<OrderDto>(`/Orders/${id}`, { method: 'GET' }, found);
  },

  createOrder: async (cmd: CreateOrderCommand): Promise<Result<string>> => {
    const newId = 'ord-' + Math.floor(1000 + Math.random() * 9000);
    const items = cmd.items.map(item => {
      const prod = mockProducts.find(p => p.id === item.productId);
      return {
        productId: item.productId,
        productName: prod?.name || 'Ürün',
        unitPrice: { amount: item.unitPriceAmount, currency: item.currency },
        quantity: item.quantity,
        totalPrice: { amount: item.unitPriceAmount * item.quantity, currency: item.currency }
      };
    });
    const totalAmount = items.reduce((acc, curr) => acc + curr.totalPrice.amount, 0);

    const userRaw = localStorage.getItem('app_user');
    const user = userRaw ? JSON.parse(userRaw) : null;

    const newOrder: OrderDto = {
      id: newId,
      userId: user?.userId || 'usr-1',
      customerName: user ? `${user.firstName} ${user.lastName}` : 'Konuk Kullanıcı',
      orderDate: new Date().toISOString().replace('T', ' ').substring(0, 16),
      status: 'Onaylandı',
      shippingAddress: cmd.shippingAddress,
      totalAmount: { amount: totalAmount, currency: 'TRY' },
      items
    };
    mockOrders.unshift(newOrder);
    return apiRequest<string>('/Orders', { method: 'POST', body: JSON.stringify(cmd) }, newId);
  },

  // USERS
  getUsers: async (): Promise<Result<UserDto[]>> => {
    return apiRequest<UserDto[]>('/Users', { method: 'GET' }, mockUsers);
  },

  createUser: async (cmd: CreateUserCommand): Promise<Result<string>> => {
    const newId = 'usr-' + (mockUsers.length + 1);
    const newUser: UserDto = {
      id: newId,
      firstName: cmd.firstName,
      lastName: cmd.lastName,
      email: cmd.email,
      role: cmd.role || 'User',
      createdAt: new Date().toISOString().split('T')[0]
    };
    mockUsers.push(newUser);
    return apiRequest<string>('/Users', { method: 'POST', body: JSON.stringify(cmd) }, newId);
  }
};
