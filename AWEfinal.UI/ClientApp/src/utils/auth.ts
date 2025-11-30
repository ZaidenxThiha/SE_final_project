import { authApi } from "./api";
import type { User } from "../types";

export const isAdmin = (user: User | null): boolean => user?.role === "admin";

export async function register(
  email: string,
  password: string,
  name: string
): Promise<User> {
  return authApi.register(email, password, name);
}

export async function login(email: string, password: string): Promise<User> {
  return authApi.login(email, password);
}

export async function logout(): Promise<void> {
  await authApi.logout();
}

export async function getCurrentUser(): Promise<User | null> {
  try {
    return await authApi.me();
  } catch {
    return null;
  }
}
