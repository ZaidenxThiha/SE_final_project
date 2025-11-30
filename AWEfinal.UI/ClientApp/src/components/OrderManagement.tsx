import { useEffect, useState } from "react";
import { Package } from "lucide-react";
import { ordersApi } from "../utils/api";
import type { Order } from "../types";
import { toast } from "sonner";
import { formatCurrency } from "../utils/currency";

export function OrderManagement() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);

  const loadOrders = async () => {
    setLoading(true);
    try {
      const data = await ordersApi.list("all");
      const sorted = data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
      setOrders(sorted);
      if (selectedOrder) {
        const refreshed = sorted.find((order) => order.id === selectedOrder.id);
        setSelectedOrder(refreshed ?? null);
      }
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to load orders");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadOrders();
  }, []);

  const handleStatusUpdate = async (orderId: number, newStatus: Order["status"]) => {
    try {
      const updated = await ordersApi.updateStatus(orderId, { status: newStatus });
      setOrders((prev) => prev.map((order) => (order.id === orderId ? updated : order)));
      setSelectedOrder(updated);
      toast.success("Order updated");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to update order");
    }
  };

  const getStatusColor = (status: Order["status"]) => {
    switch (status) {
      case "pending":
        return "bg-yellow-100 text-yellow-800";
      case "paid":
        return "bg-blue-100 text-blue-800";
      case "packaging":
        return "bg-purple-100 text-purple-800";
      case "shipped":
        return "bg-indigo-100 text-indigo-800";
      case "delivered":
        return "bg-green-100 text-green-800";
      case "cancelled":
        return "bg-red-100 text-red-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  return (
    <div>
      <h2 className="mb-6">Order Management</h2>

      <div className="grid grid-cols-3 gap-6">
        <div className="col-span-2">
          <div className="bg-white border-2 border-black rounded-lg overflow-hidden">
            <table className="w-full">
              <thead className="border-b-2 border-black" style={{ backgroundColor: "#EDEECE" }}>
                <tr>
                  <th className="px-4 py-3 text-left text-sm">Order ID</th>
                  <th className="px-4 py-3 text-left text-sm">Date</th>
                  <th className="px-4 py-3 text-left text-sm">Customer</th>
                  <th className="px-4 py-3 text-left text-sm">Total</th>
                  <th className="px-4 py-3 text-left text-sm">Status</th>
                  <th className="px-4 py-3 text-left text-sm">Action</th>
                </tr>
              </thead>
              <tbody>
                {loading ? (
                  <tr>
                    <td colSpan={6} className="px-4 py-6 text-center text-sm text-gray-500">
                      Loading orders...
                    </td>
                  </tr>
                ) : orders.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-4 py-6 text-center text-sm text-gray-500">
                      No orders yet
                    </td>
                  </tr>
                ) : (
                  orders.map((order, index) => (
                    <tr
                      key={order.id}
                      className={`cursor-pointer hover:bg-gray-100 ${index % 2 === 0 ? "bg-white" : "bg-gray-50"}`}
                      onClick={() => setSelectedOrder(order)}
                    >
                      <td className="px-4 py-3 text-sm">{order.invoiceNumber}</td>
                      <td className="px-4 py-3 text-sm">{new Date(order.createdAt).toLocaleDateString()}</td>
                      <td className="px-4 py-3 text-sm">{order.shippingAddress.fullName}</td>
                      <td className="px-4 py-3 text-sm">{formatCurrency(order.total)}</td>
                      <td className="px-4 py-3 text-sm">
                        <span className={`px-2 py-1 rounded text-xs ${getStatusColor(order.status)}`}>
                          {order.status.toUpperCase()}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-sm">
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            setSelectedOrder(order);
                          }}
                          className="text-blue-600 hover:underline text-xs"
                        >
                          View
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>

        <div>
          {selectedOrder ? (
            <div className="bg-white border-2 border-black rounded-lg p-4">
              <h3 className="mb-4">Order Details</h3>

              <div className="space-y-3 text-sm">
                <div>
                  <p className="text-gray-600">Invoice Number</p>
                  <p>{selectedOrder.invoiceNumber}</p>
                </div>

                <div>
                  <p className="text-gray-600">Customer</p>
                  <p>{selectedOrder.shippingAddress.fullName}</p>
                  <p className="text-xs text-gray-500">{selectedOrder.shippingAddress.phone}</p>
                </div>

                <div>
                  <p className="text-gray-600">Shipping Address</p>
                  <p className="text-xs">{selectedOrder.shippingAddress.address}</p>
                  <p className="text-xs">
                    {selectedOrder.shippingAddress.city}, {selectedOrder.shippingAddress.postalCode}
                  </p>
                </div>

                <div>
                  <p className="text-gray-600">Items ({selectedOrder.items.length})</p>
                  {selectedOrder.items.map((item) => (
                    <div key={item.productId} className="text-xs py-1">
                      {item.productName} x {item.quantity}
                    </div>
                  ))}
                </div>

                <div>
                  <p className="text-gray-600">Total</p>
                  <p>{formatCurrency(selectedOrder.total)}</p>
                </div>

                {selectedOrder.trackingNumber && (
                  <div>
                    <p className="text-gray-600">Tracking Number</p>
                    <p className="text-xs bg-gray-100 p-2 rounded">{selectedOrder.trackingNumber}</p>
                  </div>
                )}

                <div>
                  <p className="text-gray-600 mb-2">Update Status</p>
                  <select
                    value={selectedOrder.status}
                    onChange={(e) => handleStatusUpdate(selectedOrder.id, e.target.value as Order["status"])}
                    className="w-full border-2 border-black rounded px-2 py-1 text-sm"
                  >
                    <option value="pending">Pending</option>
                    <option value="paid">Paid</option>
                    <option value="packaging">Packaging</option>
                    <option value="shipped">Shipped</option>
                    <option value="delivered">Delivered</option>
                    <option value="cancelled">Cancelled</option>
                  </select>
                </div>
              </div>
            </div>
          ) : (
            <div className="bg-white border-2 border-black rounded-lg p-8 text-center text-gray-500">
              <Package size={48} className="mx-auto mb-2 opacity-50" />
              <p className="text-sm">Select an order to view details</p>
            </div>
          )}
        </div>
      </div>

      <div className="grid grid-cols-4 gap-4 mt-6">
        <div className="bg-white border-2 border-black rounded-lg p-4">
          <p className="text-sm text-gray-600">Total Orders</p>
          <p className="text-2xl">{orders.length}</p>
        </div>
        <div className="bg-white border-2 border-black rounded-lg p-4">
          <p className="text-sm text-gray-600">Pending</p>
          <p className="text-2xl">{orders.filter((o) => o.status === "pending" || o.status === "paid").length}</p>
        </div>
        <div className="bg-white border-2 border-black rounded-lg p-4">
          <p className="text-sm text-gray-600">In Transit</p>
          <p className="text-2xl">{orders.filter((o) => o.status === "packaging" || o.status === "shipped").length}</p>
        </div>
        <div className="bg-white border-2 border-black rounded-lg p-4">
          <p className="text-sm text-gray-600">Delivered</p>
          <p className="text-2xl">{orders.filter((o) => o.status === "delivered").length}</p>
        </div>
      </div>
    </div>
  );
}
