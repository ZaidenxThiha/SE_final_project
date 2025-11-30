import { useEffect, useMemo, useState } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import { DollarSign, Package, ShoppingCart, TrendingUp } from "lucide-react";
import { ordersApi } from "../utils/api";
import type { Order, SalesData } from "../types";
import { toast } from "sonner";
import { formatCurrency } from "../utils/currency";

type Period = "day" | "week" | "month" | "year" | "ytd";

export function Analytics() {
  const [period, setPeriod] = useState<Period>("month");
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      try {
        const data = await ordersApi.list("all");
        setOrders(data);
      } catch (error) {
        toast.error(error instanceof Error ? error.message : "Failed to load analytics");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, []);

  const stats = useMemo(() => {
    const now = new Date();
    let startDate = new Date();

    switch (period) {
      case "day":
        startDate.setDate(now.getDate() - 7);
        break;
      case "week":
        startDate.setDate(now.getDate() - 28);
        break;
      case "month":
        startDate.setMonth(now.getMonth() - 6);
        break;
      case "year":
        startDate.setFullYear(now.getFullYear() - 2);
        break;
      case "ytd":
        startDate = new Date(now.getFullYear(), 0, 1);
        break;
    }

    const filteredOrders = orders.filter((order) => {
      const orderDate = new Date(order.createdAt);
      return orderDate >= startDate && order.status !== "cancelled";
    });

    const totalRevenue = filteredOrders.reduce((sum, order) => sum + order.total, 0);
    const totalOrders = filteredOrders.length;
    const totalProducts = filteredOrders.reduce(
      (sum, order) => sum + order.items.reduce((itemSum, item) => itemSum + item.quantity, 0),
      0
    );
    const avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

    const grouped: Record<string, { revenue: number; orders: number; products: number }> = {};

    filteredOrders.forEach((order) => {
      const orderDate = new Date(order.createdAt);
      let key: string;

      switch (period) {
        case "day":
          key = orderDate.toLocaleDateString("en-US", { month: "short", day: "numeric" });
          break;
        case "week":
          const weekStart = new Date(orderDate);
          weekStart.setDate(orderDate.getDate() - orderDate.getDay());
          key = weekStart.toLocaleDateString("en-US", { month: "short", day: "numeric" });
          break;
        case "month":
        case "ytd":
          key = orderDate.toLocaleDateString("en-US", { month: "short", year: "2-digit" });
          break;
        case "year":
          key = orderDate.getFullYear().toString();
          break;
      }

      if (!grouped[key]) {
        grouped[key] = { revenue: 0, orders: 0, products: 0 };
      }

      grouped[key].revenue += order.total;
      grouped[key].orders += 1;
      grouped[key].products += order.items.reduce((sum, item) => sum + item.quantity, 0);
    });

    const chartData: SalesData[] = Object.entries(grouped)
      .map(([date, data]) => ({
        date,
        revenue: data.revenue,
        orders: data.orders,
        products: data.products,
      }))
      .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());

    return {
      totalRevenue,
      totalOrders,
      totalProducts,
      avgOrderValue,
      chartData,
    };
  }, [orders, period]);

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2>Sales Analytics</h2>

        <select
          value={period}
          onChange={(e) => setPeriod(e.target.value as Period)}
          className="border-2 border-black rounded px-4 py-2 text-sm"
          style={{ backgroundColor: "#EDEECE" }}
        >
          <option value="day">Daily (Last 7 Days)</option>
          <option value="week">Weekly (Last 4 Weeks)</option>
          <option value="month">Monthly (Last 6 Months)</option>
          <option value="year">Yearly (Last 2 Years)</option>
          <option value="ytd">Year to Date</option>
        </select>
      </div>

      {loading ? (
        <div className="text-center text-sm text-gray-600 py-12">Loading analytics...</div>
      ) : (
        <>
          <div className="grid grid-cols-4 gap-4 mb-6">
            <div className="bg-white border-2 border-black rounded-lg p-4">
              <div className="flex items-center justify-between mb-2">
                <p className="text-sm text-gray-600">Total Revenue</p>
                <DollarSign size={20} className="text-green-600" />
              </div>
              <p className="text-2xl">{formatCurrency(stats.totalRevenue)}</p>
            </div>
            <div className="bg-white border-2 border-black rounded-lg p-4">
              <div className="flex items-center justify-between mb-2">
                <p className="text-sm text-gray-600">Total Orders</p>
                <ShoppingCart size={20} className="text-blue-600" />
              </div>
              <p className="text-2xl">{stats.totalOrders}</p>
            </div>
            <div className="bg-white border-2 border-black rounded-lg p-4">
              <div className="flex items-center justify-between mb-2">
                <p className="text-sm text-gray-600">Products Sold</p>
                <Package size={20} className="text-purple-600" />
              </div>
              <p className="text-2xl">{stats.totalProducts}</p>
            </div>
            <div className="bg-white border-2 border-black rounded-lg p-4">
              <div className="flex items-center justify-between mb-2">
                <p className="text-sm text-gray-600">Avg Order Value</p>
                <TrendingUp size={20} className="text-orange-600" />
              </div>
              <p className="text-2xl">{formatCurrency(Math.round(stats.avgOrderValue))}</p>
            </div>
          </div>

          <div className="bg-white border-2 border-black rounded-lg p-6 mb-6">
            <h3 className="mb-4">Revenue Trend</h3>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={stats.chartData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis />
                <Tooltip formatter={(value: number) => formatCurrency(value)} />
                <Legend />
                <Line type="monotone" dataKey="revenue" stroke="#073634" name="Revenue (USD)" strokeWidth={2} />
              </LineChart>
            </ResponsiveContainer>
          </div>

          <div className="bg-white border-2 border-black rounded-lg p-6">
            <h3 className="mb-4">Orders & Products Sold</h3>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={stats.chartData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="orders" fill="#3b82f6" name="Orders" />
                <Bar dataKey="products" fill="#8b5cf6" name="Products" />
              </BarChart>
            </ResponsiveContainer>
          </div>

          <div className="bg-white border-2 border-black rounded-lg p-6 mt-6">
            <h3 className="mb-4">Top Selling Products</h3>
            <div className="text-sm text-gray-600">
              More detailed product analytics coming soon based on live order data.
            </div>
          </div>
        </>
      )}
    </div>
  );
}
