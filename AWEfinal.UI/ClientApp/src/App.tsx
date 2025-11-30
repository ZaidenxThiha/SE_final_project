import {
  Menu,
  Search,
  ShoppingCart,
  User as UserIcon,
  History,
  UserCircle,
  LogOut,
  LayoutDashboard,
} from "lucide-react";
import { useCallback, useEffect, useMemo, useState } from "react";
import { createPortal } from "react-dom";
import { toast, Toaster } from "sonner";

import { ProductCard } from "./components/ProductCard";
import { ProductsPage } from "./components/ProductsPage";
import { ProductDetailPage } from "./components/ProductDetailPage";
import { EnhancedCartPage } from "./components/EnhancedCartPage";
import { LoginPage } from "./components/LoginPage";
import { AdminDashboard } from "./components/AdminDashboard";
import { OrderHistory } from "./components/OrderHistory";
import { AccountPage } from "./components/AccountPage";
import { InvoicePage } from "./components/InvoicePage";

import { getCurrentUser, logout, isAdmin } from "./utils/auth";
import { cartApi, ordersApi, productsApi } from "./utils/api";
import type { Product, CartSummary, User, Order, ShippingAddress } from "./types";

type PageType =
  | "home"
  | "products"
  | "product-detail"
  | "cart"
  | "admin"
  | "orders"
  | "account"
  | "invoice";

export default function App() {
  const [currentPage, setCurrentPage] = useState<PageType>("home");
  const [showCategories, setShowCategories] = useState(false);
  const [showUserMenu, setShowUserMenu] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [currentUser, setCurrentUser] = useState<User | null>(null);
  const [currentOrder, setCurrentOrder] = useState<Order | null>(null);
  const [products, setProducts] = useState<Product[]>([]);
  const [productsLoading, setProductsLoading] = useState(false);
  const [cartSummary, setCartSummary] = useState<CartSummary | null>(null);
  const [cartLoading, setCartLoading] = useState(false);
  const [isBootstrapping, setIsBootstrapping] = useState(true);
  const [showAuth, setShowAuth] = useState(false);

  const loadProducts = useCallback(async () => {
    setProductsLoading(true);
    try {
      const response = await productsApi.list();
      setProducts(response);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to load products.");
    } finally {
      setProductsLoading(false);
    }
  }, []);

  const loadCart = useCallback(async () => {
    setCartLoading(true);
    try {
      const summary = await cartApi.get();
      setCartSummary(summary);
    } catch (error) {
      setCartSummary(null);
      toast.error(error instanceof Error ? error.message : "Failed to load cart.");
    } finally {
      setCartLoading(false);
    }
  }, []);

  useEffect(() => {
    (async () => {
      try {
        await loadProducts();
        const user = await getCurrentUser();
        if (user) {
          setCurrentUser(user);
          await loadCart();
        }
      } finally {
        setIsBootstrapping(false);
      }
    })();
  }, [loadProducts, loadCart]);

  const ensureAuthenticated = useCallback(() => {
    if (!currentUser) {
      toast.info("Please sign in to continue.");
      setShowAuth(true);
      return false;
    }
    return true;
  }, [currentUser]);

  const savedShippingAddress = useMemo(() => {
    if (!currentUser) return null;
    const hasAddress =
      currentUser.addressLine1 ||
      currentUser.addressLine2 ||
      currentUser.city ||
      currentUser.postalCode ||
      currentUser.country;
    if (!hasAddress && !currentUser.phone) return null;

    return {
      fullName: currentUser.name,
      phone: currentUser.phone ?? "",
      address: [currentUser.addressLine1, currentUser.addressLine2].filter(Boolean).join(", "),
      city: currentUser.city ?? "",
      postalCode: currentUser.postalCode ?? "",
      country: currentUser.country ?? "",
    } as ShippingAddress;
  }, [currentUser]);

  const handleProductClick = (product: Product) => {
    setSelectedProduct(product);
    setCurrentPage("product-detail");
  };

  const handleAddToCart = async (product: Product) => {
    if (!ensureAuthenticated()) {
      return;
    }
    try {
      const summary = await cartApi.add(product.id, 1);
      setCartSummary(summary);
      toast.success(`${product.name} added to cart!`);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Unable to add product to cart");
    }
  };

  const handleUpdateCartQuantity = async (productId: number, quantity: number) => {
    if (!ensureAuthenticated()) {
      return;
    }
    try {
      const summary = await cartApi.update(productId, quantity);
      setCartSummary(summary);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to update cart item");
    }
  };

  const handleRemoveFromCart = async (productId: number) => {
    if (!ensureAuthenticated()) {
      return;
    }
    try {
      const summary = await cartApi.remove(productId);
      setCartSummary(summary);
      toast.success("Item removed from cart");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Unable to remove item");
    }
  };

  const handleClearCart = async () => {
    if (!ensureAuthenticated()) {
      return;
    }
    try {
      await cartApi.clear();
      setCartSummary(null);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to clear cart");
    }
  };

  const handleCheckout = async ({
    shipping,
    paymentMethod,
  }: {
    shipping: ShippingAddress;
    paymentMethod: "cod" | "card" | "bank";
    shippingMethod: string;
  }) => {
    if (!ensureAuthenticated()) {
      return;
    }
    try {
      const order = await ordersApi.checkout({
        fullName: shipping.fullName,
        phone: shipping.phone,
        address: shipping.address,
        city: shipping.city,
        postalCode: shipping.postalCode,
        country: shipping.country,
        paymentMethod,
      });
      setCurrentOrder(order);
      setCurrentPage("invoice");
      await loadCart();
      toast.success("Order placed successfully!");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Checkout failed");
    }
  };

  const handleLogin = async (user: User) => {
    setCurrentUser(user);
    setShowUserMenu(false);
    setShowAuth(false);
    await Promise.all([loadProducts(), loadCart()]);
    if (isAdmin(user)) {
      setCurrentPage("admin");
      toast.success(`Welcome back, Admin ${user.name}!`);
    } else {
      setCurrentPage("home");
      toast.success(`Welcome back, ${user.name}!`);
    }
  };

  const handleLogout = async () => {
    await logout();
    setCurrentUser(null);
    setCartSummary(null);
    setCurrentPage("home");
    setShowUserMenu(false);
    setShowAuth(false);
    toast.success("Logged out successfully");
  };

  const handleViewInvoice = (order: Order) => {
    setCurrentOrder(order);
    setCurrentPage("invoice");
  };

  const navigateToCart = () => {
    if (!ensureAuthenticated()) {
      return;
    }
    setCurrentPage("cart");
  };

  const navigateToOrders = () => {
    if (!ensureAuthenticated()) {
      return;
    }
    setCurrentPage("orders");
  };

  const navigateToAccount = () => {
    if (!ensureAuthenticated()) {
      return;
    }
    setCurrentPage("account");
  };

  const navigateToAdmin = () => {
    if (!ensureAuthenticated()) {
      return;
    }
    setCurrentPage("admin");
  };

  const categories = useMemo(
    () => [
      "Smartphones",
      "Laptops",
      "Tablets",
      "Headphones",
      "Smart TVs",
      "Cameras",
      "Smartwatches",
      "Gaming Consoles",
      "Speakers",
      "Monitors",
    ],
    []
  );

  if (isBootstrapping) {
    return (
      <div className="min-h-screen flex items-center justify-center" style={{ backgroundColor: "#EDEECE" }}>
        <p className="text-gray-600 text-sm">Loading...</p>
      </div>
    );
  }

  if (currentUser && isAdmin(currentUser) && currentPage === "admin") {
    return <AdminDashboard user={currentUser} onLogout={handleLogout} />;
  }

  if (currentPage === "invoice" && currentOrder) {
    return <InvoicePage order={currentOrder} onContinue={() => setCurrentPage("home")} />;
  }

  if (currentPage === "orders" && currentUser) {
    return (
      <>
        <Toaster position="top-right" />
        <OrderHistory
          user={currentUser}
          onViewInvoice={handleViewInvoice}
          onBackToHome={() => setCurrentPage("home")}
        />
      </>
    );
  }

  if (currentPage === "account" && currentUser) {
    return (
      <>
        <Toaster position="top-right" />
        <AccountPage
          user={currentUser}
          onLogout={handleLogout}
          onViewOrders={() => setCurrentPage("orders")}
          onBackToHome={() => setCurrentPage("home")}
          onUserUpdate={(updated) => setCurrentUser(updated)}
        />
      </>
    );
  }

  return (
    <>
      <Toaster position="top-right" />
      <div className="min-h-screen" style={{ backgroundColor: "#EDEECE" }}>
        <nav className="text-white shadow-lg" style={{ backgroundColor: "#073634" }}>
          <div className="max-w-7xl mx-auto px-6 py-4">
            <div className="flex items-center gap-6">
              <div className="flex items-center gap-4">
                <div className="relative">
                  <button
                    onClick={() => setShowCategories(!showCategories)}
                    className="p-3 rounded-full border border-white/30 bg-white/10 hover:bg-white/20 transition-colors"
                    aria-label="Toggle categories"
                  >
                    <Menu className="w-5 h-5" />
                  </button>

                  {showCategories && (
                    <div className="absolute top-full left-0 mt-2 w-56 bg-white border-2 border-black rounded-lg shadow-lg z-50">
                      <div className="py-2">
                        <button
                          className="w-full text-left px-4 py-2 text-sm text-black hover:bg-gray-100 transition-colors"
                          onClick={() => {
                            setSelectedCategory(null);
                            setCurrentPage("products");
                            setShowCategories(false);
                          }}
                        >
                          All Categories
                        </button>
                        {categories.map((category) => (
                          <button
                            key={category}
                            onClick={() => {
                              setSelectedCategory(category);
                              setCurrentPage("products");
                              setShowCategories(false);
                            }}
                            className="w-full text-left px-4 py-2 text-sm text-black hover:bg-gray-100 transition-colors"
                          >
                            {category}
                          </button>
                        ))}
                      </div>
                    </div>
                  )}
                </div>

                <div className="flex flex-col leading-tight cursor-pointer" onClick={() => setCurrentPage("home")}>
                  <div className="text-2xl font-semibold tracking-[0.3em] uppercase">AWE</div>
                  <span className="text-xs uppercase tracking-wide text-green-100">Electronics</span>
                </div>
              </div>

              <div className="flex-1">
                <div className="relative max-w-3xl mx-auto">
                  <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-500 pointer-events-none" />
                  <input
                    type="text"
                    placeholder="Looking for something? We got you!"
                    className="w-full pl-12 pr-12 py-3 rounded-full bg-white text-sm text-gray-800 shadow-inner focus:outline-none text-center"
                  />
                </div>
                <div className="text-center text-white text-sm mt-2">
                  Trusted Quality. Simple Clicks. Delivered to Your Door.
                </div>
              </div>

              <div className="flex items-center gap-4 ml-auto">
              {currentUser && (
                <span className="hidden sm:inline text-xs font-semibold text-white uppercase tracking-wide">
                  {currentUser.name}
                </span>
              )}
              <button onClick={navigateToCart} className="relative hover:opacity-90">
                <ShoppingCart className="w-6 h-6" />
                  {cartSummary?.totalItems ? (
                    <span className="absolute -top-1 -right-1 bg-red-600 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                      {cartSummary.totalItems}
                    </span>
                  ) : null}
                </button>

                {currentUser ? (
                  <div className="relative flex items-center gap-2">
                    <span className="hidden sm:inline text-xs uppercase tracking-wide text-white">
                      {currentUser.name}
                    </span>
                    <button onClick={() => setShowUserMenu(!showUserMenu)} className="hover:opacity-90">
                      <UserIcon className="w-6 h-6" />
                    </button>

                    {showUserMenu && (
                      <div className="absolute top-full right-0 mt-2 w-48 bg-white border-2 border-black rounded-lg shadow-lg z-50">
                        <div className="p-3 border-b border-gray-200">
                          <p className="text-sm text-black">{currentUser.name}</p>
                          <p className="text-xs text-gray-600">{currentUser.email}</p>
                        </div>
                        <div className="py-2">
                          {isAdmin(currentUser) && (
                            <button
                              onClick={() => {
                                navigateToAdmin();
                                setShowUserMenu(false);
                              }}
                              className="w-full text-left px-4 py-2 text-sm text-black hover:bg-gray-100 flex items-center gap-2"
                            >
                              <LayoutDashboard size={14} />
                              Admin Dashboard
                            </button>
                          )}
                          <button
                            onClick={() => {
                              navigateToOrders();
                              setShowUserMenu(false);
                            }}
                            className="w-full text-left px-4 py-2 text-sm text-black hover:bg-gray-100 flex items-center gap-2"
                          >
                            <History size={14} />
                            Orders
                          </button>
                          <button
                            onClick={() => {
                              navigateToAccount();
                              setShowUserMenu(false);
                            }}
                            className="w-full text-left px-4 py-2 text-sm text-black hover:bg-gray-100 flex items-center gap-2"
                          >
                            <UserCircle size={14} />
                            Account
                          </button>
                          <button
                            onClick={handleLogout}
                            className="w-full text-left px-4 py-2 text-sm text-black hover:bg-gray-100 flex items-center gap-2"
                          >
                            <LogOut size={14} />
                            Logout
                          </button>
                        </div>
                      </div>
                    )}
                  </div>
                ) : (
                  <button
                    onClick={() => setShowAuth(true)}
                    className="p-2 border-2 border-white rounded-full hover:opacity-90"
                    aria-label="Sign in"
                  >
                    <UserIcon className="w-5 h-5" />
                  </button>
                )}
              </div>
            </div>
          </div>
        </nav>

        <main className="max-w-7xl mx-auto px-6 py-6">
          {currentPage === "products" ? (
            <ProductsPage
              products={products}
              selectedCategory={selectedCategory}
              isLoading={productsLoading}
              onProductClick={handleProductClick}
              onAddToCart={handleAddToCart}
            />
          ) : currentPage === "product-detail" && selectedProduct ? (
            <ProductDetailPage
              product={selectedProduct}
              onBack={() => setCurrentPage("home")}
              onAddToCart={handleAddToCart}
              onBuyNow={(product) => {
                handleAddToCart(product);
                setCurrentPage("cart");
              }}
            />
          ) : currentPage === "cart" && currentUser ? (
            <EnhancedCartPage
              cart={cartSummary}
              isLoading={cartLoading}
              onUpdateQuantity={handleUpdateCartQuantity}
              onRemoveItem={handleRemoveFromCart}
              onClearCart={handleClearCart}
              onCheckout={handleCheckout}
              user={currentUser}
              savedShipping={savedShippingAddress}
            />
          ) : currentPage === "cart" ? (
            <div className="text-center text-sm text-gray-600 py-12">Please sign in to view your cart.</div>
          ) : (
            <>
              <section>
                <div className="flex justify-between items-center mb-6">
                  <h2 className="text-xl">Popular Products</h2>
                  <button
                    className="text-sm underline"
                    onClick={() => {
                      setSelectedCategory(null);
                      setCurrentPage("products");
                    }}
                  >
                    View all
                  </button>
                </div>
                {productsLoading ? (
                  <div className="text-center text-sm text-gray-600 py-12">Loading products...</div>
                ) : products.length === 0 ? (
                  <div className="text-center text-sm text-gray-600 py-12 border-2 border-dashed border-gray-300 rounded">
                    No products available.
                  </div>
                ) : (
                  <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
                    {products.slice(0, 8).map((product) => (
                      <ProductCard key={product.id} product={product} onClick={() => handleProductClick(product)} onAddToCart={handleAddToCart} />
                    ))}
                  </div>
                )}
              </section>
            </>
          )}
        </main>

        <footer className="border-t-2 border-black mt-12">
          <div className="max-w-7xl mx-auto px-4 py-4">
            <div className="border border-black bg-gray-200 px-3 py-1 text-sm inline-block">Contact Us: 03-00000000</div>
          </div>
        </footer>
      </div>
      {showAuth &&
        typeof document !== "undefined" &&
        createPortal(
          <div className="auth-overlay" onClick={() => setShowAuth(false)}>
            <div className="auth-modal" onClick={(e) => e.stopPropagation()}>
              <LoginPage onLogin={handleLogin} isModal onClose={() => setShowAuth(false)} />
            </div>
          </div>,
          document.body
        )}
    </>
  );
}
