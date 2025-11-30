import { useState } from 'react';
import { User } from '../types';
import { ProductManagement } from './ProductManagement';
import { OrderManagement } from './OrderManagement';
import { Analytics } from './Analytics';
import { Package, BarChart3, ShoppingBag, Settings } from 'lucide-react';

interface AdminDashboardProps {
  user: User;
  onLogout: () => void;
}

export function AdminDashboard({ user, onLogout }: AdminDashboardProps) {
  const [activeTab, setActiveTab] = useState<'products' | 'orders' | 'analytics'>('products');

  return (
    <div className="min-h-screen" style={{ backgroundColor: '#EDEECE' }}>
      {/* Header */}
      <header className="text-white py-4 border-b-2 border-black" style={{ backgroundColor: '#073634' }}>
        <div className="max-w-7xl mx-auto px-4 flex justify-between items-center">
          <div>
            <h1>Admin Dashboard</h1>
            <p className="text-sm opacity-90">Welcome, {user.name}</p>
          </div>
          <button
            onClick={onLogout}
            className="px-4 py-2 bg-white text-black border-2 border-black rounded hover:bg-gray-100 text-sm"
          >
            Logout
          </button>
        </div>
      </header>

      {/* Navigation Tabs */}
      <div className="border-b-2 border-black bg-white">
        <div className="max-w-7xl mx-auto px-4">
          <div className="flex gap-1">
            <button
              onClick={() => setActiveTab('products')}
              className={`px-6 py-3 text-sm border-2 border-black border-b-0 flex items-center gap-2 ${
                activeTab === 'products' ? 'bg-white -mb-0.5' : 'bg-gray-200'
              }`}
            >
              <ShoppingBag size={18} />
              Product Management
            </button>
            <button
              onClick={() => setActiveTab('orders')}
              className={`px-6 py-3 text-sm border-2 border-black border-b-0 flex items-center gap-2 ${
                activeTab === 'orders' ? 'bg-white -mb-0.5' : 'bg-gray-200'
              }`}
            >
              <Package size={18} />
              Order Management
            </button>
            <button
              onClick={() => setActiveTab('analytics')}
              className={`px-6 py-3 text-sm border-2 border-black border-b-0 flex items-center gap-2 ${
                activeTab === 'analytics' ? 'bg-white -mb-0.5' : 'bg-gray-200'
              }`}
            >
              <BarChart3 size={18} />
              Analytics
            </button>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="max-w-7xl mx-auto px-4 py-6">
        {activeTab === 'products' && <ProductManagement />}
        {activeTab === 'orders' && <OrderManagement />}
        {activeTab === 'analytics' && <Analytics />}
      </div>
    </div>
  );
}
