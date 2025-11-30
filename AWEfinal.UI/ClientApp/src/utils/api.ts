import type {
  CartSummary,
  Order,
  Product,
  User,
  UserProfileResponse,
} from "../types";

const API_BASE_URL =
  (import.meta.env.VITE_API_BASE_URL as string | undefined)?.replace(/\/$/, "") ||
  "";

async function apiRequest<T>(path: string, options: RequestInit = {}): Promise<T> {
  const headers = new Headers(options.headers);
  const hasBody = options.body !== undefined;
  const isFormData =
    typeof FormData !== "undefined" && options.body instanceof FormData;

  if (hasBody && !isFormData && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  if (!headers.has("Accept")) {
    headers.set("Accept", "application/json");
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    credentials: "include",
    ...options,
    headers,
  });

  if (!response.ok) {
    let message = "Request failed";
    try {
      const errorBody = await response.json();
      if (errorBody?.message) {
        message = errorBody.message;
      }
    } catch {
      // Ignore JSON parsing issues for error bodies
    }
    throw new Error(message);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  const contentType = response.headers.get("content-type");
  if (contentType?.includes("application/json")) {
    return (await response.json()) as T;
  }

  const text = await response.text();
  return text as unknown as T;
}

export const authApi = {
  me: () => apiRequest<User>("/api/auth/me"),
  login: (email: string, password: string) =>
    apiRequest<User>("/api/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    }),
  register: (email: string, password: string, name: string) =>
    apiRequest<User>("/api/auth/register", {
      method: "POST",
      body: JSON.stringify({ email, password, name }),
    }),
  logout: () =>
    apiRequest<void>("/api/auth/logout", {
      method: "POST",
    }),
};

export const productsApi = {
  list: (params?: { category?: string; search?: string; sort?: string }) => {
    const query = new URLSearchParams();
    if (params?.category) query.set("category", params.category);
    if (params?.search) query.set("search", params.search);
    if (params?.sort) query.set("sort", params.sort);
    const qs = query.toString();
    return apiRequest<Product[]>(`/api/products${qs ? `?${qs}` : ""}`);
  },
  detail: (id: number) => apiRequest<Product>(`/api/products/${id}`),
  create: (payload: Partial<Product>) =>
    apiRequest<Product>("/api/products", {
      method: "POST",
      body: JSON.stringify(payload),
    }),
  update: (id: number, payload: Partial<Product>) =>
    apiRequest<Product>(`/api/products/${id}`, {
      method: "PUT",
      body: JSON.stringify(payload),
    }),
  remove: (id: number) =>
    apiRequest<void>(`/api/products/${id}`, {
      method: "DELETE",
    }),
  uploadImage: (id: number, file: File) => {
    const formData = new FormData();
    formData.append("file", file);
    return apiRequest<{ images: string[] }>(`/api/products/${id}/images`, {
      method: "POST",
      body: formData,
    });
  },
};

export const cartApi = {
  get: () => apiRequest<CartSummary>("/api/cart"),
  add: (productId: number, quantity = 1) =>
    apiRequest<CartSummary>("/api/cart", {
      method: "POST",
      body: JSON.stringify({ productId, quantity }),
    }),
  update: (productId: number, quantity: number) =>
    apiRequest<CartSummary>(`/api/cart/${productId}`, {
      method: "PUT",
      body: JSON.stringify({ quantity }),
    }),
  remove: (productId: number) =>
    apiRequest<CartSummary>(`/api/cart/${productId}`, {
      method: "DELETE",
    }),
  clear: () =>
    apiRequest<void>("/api/cart", {
      method: "DELETE",
    }),
};

export const ordersApi = {
  list: (scope: "mine" | "all" = "mine") =>
    apiRequest<Order[]>(`/api/orders?scope=${scope}`),
  checkout: (payload: {
    fullName: string;
    phone: string;
    address: string;
    city: string;
    postalCode: string;
    country: string;
    paymentMethod: string;
  }) =>
    apiRequest<Order>("/api/orders", {
      method: "POST",
      body: JSON.stringify(payload),
    }),
  updateStatus: (id: number, payload: { status: string; trackingNumber?: string }) =>
    apiRequest<Order>(`/api/orders/${id}/status`, {
      method: "PUT",
      body: JSON.stringify(payload),
    }),
};

export const profileApi = {
  get: () => apiRequest<UserProfileResponse>("/api/profile"),
  update: (payload: {
    name: string;
    phone?: string;
    addressLine1?: string;
    addressLine2?: string;
    city?: string;
    postalCode?: string;
    country?: string;
  }) =>
    apiRequest<User>("/api/profile", {
      method: "PUT",
      body: JSON.stringify(payload),
    }),
  changePassword: (currentPassword: string, newPassword: string) =>
    apiRequest<void>("/api/profile/password", {
      method: "PUT",
      body: JSON.stringify({ currentPassword, newPassword }),
    }),
};

export { apiRequest };
