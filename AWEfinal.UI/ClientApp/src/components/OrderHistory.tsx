import { useEffect, useState } from "react";
import { Package, Eye, Home } from "lucide-react";
import { ordersApi } from "../utils/api";
import type { User, Order } from "../types";
import { toast } from "sonner";
import { formatCurrency } from "../utils/currency";

interface OrderHistoryProps {
  user: User;
  onViewInvoice: (order: Order) => void;
  onBackToHome?: () => void;
}

export function OrderHistory({ user, onViewInvoice, onBackToHome }: OrderHistoryProps) {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      try {
        const data = await ordersApi.list("mine");
        setOrders(
          data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
        );
      } catch (error) {
        toast.error(error instanceof Error ? error.message : "Failed to load orders");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [user.id]);

  const getStatusColor = (status: Order['status']) => {
    switch (status) {
      case 'pending': return 'bg-yellow-100 text-yellow-800';
      case 'paid': return 'bg-blue-100 text-blue-800';
      case 'packaging': return 'bg-purple-100 text-purple-800';
      case 'shipped': return 'bg-indigo-100 text-indigo-800';
      case 'delivered': return 'bg-green-100 text-green-800';
      case 'cancelled': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  return (
    <div className="min-h-screen" style={{ backgroundColor: '#EDEECE' }}>
      <div className="max-w-7xl mx-auto px-6 py-6">
        <div className="flex justify-between items-center mb-6">
          <h1>Order History</h1>
          {onBackToHome && (
            <button
              onClick={onBackToHome}
              className="px-4 py-2 border-2 border-black rounded flex items-center gap-2 text-sm text-white hover:opacity-90 transition-opacity"
              style={{ backgroundColor: '#073634' }}
            >
              <Home size={16} />
              Back to Home
            </button>
          )}
        </div>

        {loading ? (
          <div className="bg-white border-2 border-black rounded-lg p-12 text-center">
            <p className="text-gray-600 text-sm">Loading orders...</p>
          </div>
        ) : orders.length === 0 ? (
          <div className="bg-white border-2 border-black rounded-lg p-12 text-center">
            <Package size={48} className="mx-auto mb-4 text-gray-400" />
            <p className="text-gray-600">No orders yet</p>
            <p className="text-sm text-gray-500 mt-1">Your order history will appear here</p>
          </div>
        ) : (
          <div className="space-y-4">
            {orders.map((order) => (
              <div key={order.id} className="bg-white border-2 border-black rounded-lg p-6">
                <div className="flex justify-between items-start mb-4">
                  <div>
                    <h3 className="mb-1">Order {order.invoiceNumber}</h3>
                    <p className="text-sm text-gray-600">
                      Placed on {new Date(order.createdAt).toLocaleDateString()} at {new Date(order.createdAt).toLocaleTimeString()}
                    </p>
                  </div>
                  <div className="text-right">
                    <span className={`px-3 py-1 rounded text-xs ${getStatusColor(order.status)}`}>
                      {order.status.toUpperCase()}
                    </span>
                    {order.trackingNumber && (
                      <p className="text-xs text-gray-600 mt-2">
                        Tracking: {order.trackingNumber}
                      </p>
                    )}
                  </div>
                </div>

                <div className="border-t border-gray-200 pt-4">
                  <div className="space-y-2 mb-4">
                    {order.items.map((item, idx) => (
                      <div key={idx} className="flex justify-between text-sm">
                        <span>
                          {item.productName} <span className="text-gray-600">x {item.quantity}</span>
                        </span>
                        <span>{formatCurrency(item.subtotal)}</span>
                      </div>
                    ))}
                  </div>

                  <div className="flex justify-between items-center border-t border-gray-200 pt-4">
                    <div>
                      <p className="text-sm text-gray-600">Total Amount</p>
                      <p>{formatCurrency(order.total)}</p>
                    </div>
                    <button
                      onClick={() => onViewInvoice(order)}
                      className="px-4 py-2 border-2 border-black rounded flex items-center gap-2 text-sm hover:bg-gray-100"
                      style={{ backgroundColor: '#EDEECE' }}
                    >
                      <Eye size={16} />
                      View Invoice
                    </button>
                  </div>
                </div>

                {/* Order Progress */}
                <div className="mt-6 pt-6 border-t border-gray-200">
                  <div className="flex items-center justify-between text-xs">
                    <div className={`flex flex-col items-center ${['pending', 'paid', 'packaging', 'shipped', 'delivered'].includes(order.status) ? 'text-green-600' : 'text-gray-400'}`}>
                      <div className={`w-8 h-8 rounded-full border-2 flex items-center justify-center mb-1 ${['pending', 'paid', 'packaging', 'shipped', 'delivered'].includes(order.status) ? 'border-green-600 bg-green-100' : 'border-gray-300'}`}>
                        ✓
                      </div>
                      <span>Ordered</span>
                    </div>
                    <div className={`flex-1 h-0.5 ${['paid', 'packaging', 'shipped', 'delivered'].includes(order.status) ? 'bg-green-600' : 'bg-gray-300'}`}></div>
                    
                    <div className={`flex flex-col items-center ${['paid', 'packaging', 'shipped', 'delivered'].includes(order.status) ? 'text-green-600' : 'text-gray-400'}`}>
                      <div className={`w-8 h-8 rounded-full border-2 flex items-center justify-center mb-1 ${['paid', 'packaging', 'shipped', 'delivered'].includes(order.status) ? 'border-green-600 bg-green-100' : 'border-gray-300'}`}>
                        {['paid', 'packaging', 'shipped', 'delivered'].includes(order.status) ? '✓' : '2'}
                      </div>
                      <span>Paid</span>
                    </div>
                    <div className={`flex-1 h-0.5 ${['packaging', 'shipped', 'delivered'].includes(order.status) ? 'bg-green-600' : 'bg-gray-300'}`}></div>
                    
                    <div className={`flex flex-col items-center ${['packaging', 'shipped', 'delivered'].includes(order.status) ? 'text-green-600' : 'text-gray-400'}`}>
                      <div className={`w-8 h-8 rounded-full border-2 flex items-center justify-center mb-1 ${['packaging', 'shipped', 'delivered'].includes(order.status) ? 'border-green-600 bg-green-100' : 'border-gray-300'}`}>
                        {['packaging', 'shipped', 'delivered'].includes(order.status) ? '✓' : '3'}
                      </div>
                      <span>Packaging</span>
                    </div>
                    <div className={`flex-1 h-0.5 ${['shipped', 'delivered'].includes(order.status) ? 'bg-green-600' : 'bg-gray-300'}`}></div>
                    
                    <div className={`flex flex-col items-center ${['shipped', 'delivered'].includes(order.status) ? 'text-green-600' : 'text-gray-400'}`}>
                      <div className={`w-8 h-8 rounded-full border-2 flex items-center justify-center mb-1 ${['shipped', 'delivered'].includes(order.status) ? 'border-green-600 bg-green-100' : 'border-gray-300'}`}>
                        {['shipped', 'delivered'].includes(order.status) ? '✓' : '4'}
                      </div>
                      <span>Shipped</span>
                    </div>
                    <div className={`flex-1 h-0.5 ${order.status === 'delivered' ? 'bg-green-600' : 'bg-gray-300'}`}></div>
                    
                    <div className={`flex flex-col items-center ${order.status === 'delivered' ? 'text-green-600' : 'text-gray-400'}`}>
                      <div className={`w-8 h-8 rounded-full border-2 flex items-center justify-center mb-1 ${order.status === 'delivered' ? 'border-green-600 bg-green-100' : 'border-gray-300'}`}>
                        {order.status === 'delivered' ? '✓' : '5'}
                      </div>
                      <span>Delivered</span>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
