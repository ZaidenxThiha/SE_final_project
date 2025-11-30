export type UserRole = "customer" | "admin";

export interface User {
  id: number;
  email: string;
  name: string;
  role: UserRole;
  createdAt: string;
  phone?: string | null;
  addressLine1?: string | null;
  addressLine2?: string | null;
  city?: string | null;
  postalCode?: string | null;
  country?: string | null;
}

export interface Product {
  id: number;
  name: string;
  price: number;
  category: string;
  description: string;
  storage?: string;
  colors: string[];
  sizes?: string[];
  rating: number;
  images: string[];
  features: string[];
  inStock: boolean;
  stockQuantity: number;
  createdAt: string;
}

export interface CartItem {
  productId: number;
  productName: string;
  price: number;
  quantity: number;
  imageUrl: string;
  subtotal: number;
}

export interface CartSummary {
  items: CartItem[];
  totalItems: number;
  total: number;
}

export interface ShippingAddress {
  fullName: string;
  phone: string;
  address: string;
  city: string;
  postalCode: string;
  country: string;
}

export interface OrderItem {
  productId: number;
  productName: string;
  quantity: number;
  price: number;
  subtotal: number;
  imageUrl?: string | null;
}

export type OrderStatus =
  | "pending"
  | "paid"
  | "packaging"
  | "shipped"
  | "delivered"
  | "cancelled";

export interface Order {
  id: number;
  userId: number;
  orderNumber: string;
  invoiceNumber: string;
  status: OrderStatus;
  total: number;
  paymentMethod: string;
  shippingAddress: ShippingAddress;
  trackingNumber?: string;
  createdAt: string;
  updatedAt: string;
  items: OrderItem[];
}

export interface SalesData {
  date: string;
  revenue: number;
  orders: number;
  products: number;
}

export interface UserStats {
  totalOrders: number;
  deliveredOrders: number;
  inProgressOrders: number;
  totalSpent: number;
}

export interface UserProfileResponse {
  user: User;
  stats: UserStats;
}
