import { useMemo, useState } from "react";
import { Minus, Plus, Trash2 } from "lucide-react";
import type { CartSummary, ShippingAddress, User } from "../types";
import { formatCurrency } from "../utils/currency";

type ShippingSpeed = "standard" | "fast" | "super";
type PaymentMethod = "cod" | "card" | "bank";

interface EnhancedCartPageProps {
  cart: CartSummary | null;
  isLoading: boolean;
  onUpdateQuantity: (productId: number, quantity: number) => Promise<void> | void;
  onRemoveItem: (productId: number) => Promise<void> | void;
  onClearCart: () => Promise<void> | void;
  onCheckout: (payload: {
    shipping: ShippingAddress;
    paymentMethod: PaymentMethod;
    shippingMethod: ShippingSpeed;
  }) => Promise<void>;
  user: User | null;
  savedShipping?: ShippingAddress | null;
}

export function EnhancedCartPage({
  cart,
  isLoading,
  onUpdateQuantity,
  onRemoveItem,
  onClearCart,
  onCheckout,
  user,
  savedShipping,
}: EnhancedCartPageProps) {
  const [shippingMethod, setShippingMethod] = useState<ShippingSpeed>("standard");
  const [shippingAddress, setShippingAddress] = useState<ShippingAddress>({
    fullName: user?.name || "",
    phone: "",
    address: "",
    city: "",
    postalCode: "",
    country: "Vietnam",
  });
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>("cod");
  const [isProcessing, setIsProcessing] = useState(false);

  const subtotal = useMemo(() => {
    return cart?.items?.reduce((sum, item) => sum + item.subtotal, 0) ?? 0;
  }, [cart]);

  const shippingFee =
    shippingMethod === "fast" ? 50 : shippingMethod === "super" ? 30 : 0;
  const vat = subtotal * 0.1;
  const total = subtotal + shippingFee + vat;

  const handleCheckout = async () => {
    if (!user) {
      alert("Please login to checkout");
      return;
    }

    if (!shippingAddress.fullName || !shippingAddress.phone || !shippingAddress.address) {
      alert("Please fill in all shipping details");
      return;
    }

    setIsProcessing(true);
    try {
      await onCheckout({
        shipping: shippingAddress,
        paymentMethod,
        shippingMethod,
      });
    } finally {
      setIsProcessing(false);
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center" style={{ backgroundColor: "#EDEECE" }}>
        <p className="text-gray-600 text-sm">Loading cart...</p>
      </div>
    );
  }

  if (!cart || cart.items.length === 0) {
    return (
      <div className="min-h-screen flex items-center justify-center" style={{ backgroundColor: "#EDEECE" }}>
        <div className="text-center">
          <h2 className="mb-4">Your cart is empty</h2>
          <p className="text-gray-600">Add some products to get started!</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen" style={{ backgroundColor: "#EDEECE" }}>
      <div className="max-w-7xl mx-auto px-6 py-6">
        <h1 className="mb-6">Shopping Cart & Checkout</h1>

        <div className="flex gap-6">
          <div className="flex-1 space-y-6">
            <div className="bg-white border-2 border-black rounded-lg p-6">
              <div className="flex items-center justify-between mb-4">
                <h3>Cart Items ({cart.items.length})</h3>
                {cart.items.length > 0 && (
                  <button onClick={() => onClearCart()} className="text-xs text-red-600 hover:underline">
                    Clear cart
                  </button>
                )}
              </div>

              {cart.items.map((item) => (
                <div key={item.productId} className="flex gap-4 py-4 border-b border-gray-200 last:border-0">
                  <div className="w-20 h-20 bg-gray-200 border border-black rounded flex items-center justify-center text-xs text-gray-500 overflow-hidden">
                    <img
                      src={item.imageUrl || "/images/products/placeholder.jpg"}
                      alt={item.productName}
                      className="w-full h-full object-cover"
                      onError={(e) => {
                        (e.currentTarget as HTMLImageElement).src = "/images/products/placeholder.jpg";
                      }}
                    />
                  </div>

                  <div className="flex-1">
                    <h4 className="text-sm mb-1">{item.productName}</h4>
                    <p className="text-sm">{formatCurrency(item.price)}</p>
                  </div>

                  <div className="flex flex-col items-end gap-2">
                    <button
                      onClick={() => onRemoveItem(item.productId)}
                      className="text-red-600 hover:bg-red-50 p-1 rounded"
                    >
                      <Trash2 size={16} />
                    </button>

                    <div className="flex items-center gap-2 border border-black rounded">
                      <button
                        onClick={() => onUpdateQuantity(item.productId, item.quantity - 1)}
                        className="p-1 hover:bg-gray-100"
                        disabled={item.quantity <= 1}
                      >
                        <Minus size={16} />
                      </button>
                      <span className="w-8 text-center text-sm">{item.quantity}</span>
                      <button
                        onClick={() => onUpdateQuantity(item.productId, item.quantity + 1)}
                        className="p-1 hover:bg-gray-100"
                      >
                        <Plus size={16} />
                      </button>
                    </div>
                  </div>
                </div>
              ))}
            </div>

            <div className="bg-white border-2 border-black rounded-lg p-6">
              <div className="flex items-center justify-between mb-4">
                <h3>Shipping Information</h3>
                {savedShipping && (
                  <button
                    onClick={() => setShippingAddress(savedShipping)}
                    className="text-xs px-3 py-1 border border-black rounded hover:bg-gray-100"
                  >
                    Use Saved Address
                  </button>
                )}
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="col-span-2">
                  <label className="block text-sm mb-1">Full Name</label>
                  <input
                    type="text"
                    value={shippingAddress.fullName}
                    onChange={(e) => setShippingAddress({ ...shippingAddress, fullName: e.target.value })}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm mb-1">Phone Number</label>
                  <input
                    type="tel"
                    value={shippingAddress.phone}
                    onChange={(e) => setShippingAddress({ ...shippingAddress, phone: e.target.value })}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm mb-1">City</label>
                  <input
                    type="text"
                    value={shippingAddress.city}
                    onChange={(e) => setShippingAddress({ ...shippingAddress, city: e.target.value })}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                    required
                  />
                </div>

                <div className="col-span-2">
                  <label className="block text-sm mb-1">Address</label>
                  <textarea
                    value={shippingAddress.address}
                    onChange={(e) => setShippingAddress({ ...shippingAddress, address: e.target.value })}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                    rows={2}
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm mb-1">Postal Code</label>
                  <input
                    type="text"
                    value={shippingAddress.postalCode}
                    onChange={(e) => setShippingAddress({ ...shippingAddress, postalCode: e.target.value })}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                  />
                </div>

                <div>
                  <label className="block text-sm mb-1">Country</label>
                  <input
                    type="text"
                    value={shippingAddress.country}
                    onChange={(e) => setShippingAddress({ ...shippingAddress, country: e.target.value })}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                  />
                </div>

                <div className="col-span-2">
                  <label className="block text-sm mb-1">Shipping Method</label>
                  <select
                    value={shippingMethod}
                    onChange={(e) => setShippingMethod(e.target.value as ShippingSpeed)}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                  >
                    <option value="standard">Standard (Free)</option>
                    <option value="fast">Expedited (+$50.00)</option>
                    <option value="super">Same Day (+$30.00)</option>
                  </select>
                </div>
              </div>
            </div>
          </div>

          <div className="w-96">
            <div className="bg-white border-2 border-black rounded-lg p-6">
              <h3 className="mb-4">Order Summary</h3>

              <div className="space-y-3">
                <div className="flex justify-between text-sm">
                  <span>Subtotal</span>
                  <span>{formatCurrency(subtotal)}</span>
                </div>

                <div className="flex justify-between text-sm">
                  <span>Shipping</span>
                  <span>{formatCurrency(shippingFee)}</span>
                </div>

                <div className="flex justify-between text-sm">
                  <span>VAT (10%)</span>
                  <span>{formatCurrency(vat)}</span>
                </div>

                <div className="flex justify-between text-lg font-semibold pt-3 border-t border-gray-200">
                  <span>Total</span>
                  <span>{formatCurrency(total)}</span>
                </div>
              </div>

              <div className="mt-6">
                <label className="block text-sm mb-2">Payment Method</label>
                <div className="space-y-2">
                  {["cod", "card", "bank"].map((method) => (
                    <label key={method} className="flex items-center gap-2 text-sm">
                      <input
                        type="radio"
                        name="paymentMethod"
                        value={method}
                        checked={paymentMethod === method}
                        onChange={(e) => setPaymentMethod(e.target.value as PaymentMethod)}
                      />
                      <span className="capitalize">
                        {method === "cod" ? "Cash on Delivery" : method === "card" ? "Credit / Debit Card" : "Bank Transfer"}
                      </span>
                    </label>
                  ))}
                </div>
              </div>

              <div className="mt-6">
                <button
                  onClick={handleCheckout}
                  className="w-full py-3 text-white rounded disabled:opacity-60"
                  style={{ backgroundColor: "#073634" }}
                  disabled={isProcessing}
                >
                  {isProcessing ? "Processing..." : "Place Order"}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
