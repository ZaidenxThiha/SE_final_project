import { Printer } from "lucide-react";
import type { Order } from "../types";
import { formatCurrency } from "../utils/currency";

interface InvoicePageProps {
  order: Order;
  onContinue: () => void;
}

export function InvoicePage({ order, onContinue }: InvoicePageProps) {
  const lineSubtotal = order.items.reduce((sum, item) => sum + item.subtotal, 0);
  const vat = lineSubtotal * 0.1;
  const shipping = Math.max(order.total - lineSubtotal - vat, 0);

  const handlePrint = () => {
    window.print();
  };

  return (
    <div className="min-h-screen py-8" style={{ backgroundColor: '#EDEECE' }}>
      <div className="max-w-4xl mx-auto px-6">
        {/* Print Button (hidden when printing) */}
        <div className="flex justify-between items-center mb-6 print:hidden">
          <h1>Invoice</h1>
          <div className="flex gap-2">
            <button
              onClick={handlePrint}
              className="px-4 py-2 border-2 border-black rounded flex items-center gap-2 text-sm hover:bg-white"
              style={{ backgroundColor: '#EDEECE' }}
            >
              <Printer size={18} />
              Print Invoice
            </button>
            <button
              onClick={onContinue}
              className="px-6 py-2 rounded text-white text-sm"
              style={{ backgroundColor: '#073634' }}
            >
              Continue Shopping
            </button>
          </div>
        </div>

        {/* Invoice Document */}
        <div className="bg-white border-2 border-black rounded-lg p-8">
          {/* Header */}
          <div className="border-b-2 border-black pb-6 mb-6">
            <div className="flex justify-between items-start">
              <div>
                <h2 className="mb-2">AWE Electronics</h2>
                <p className="text-sm text-gray-600">Glenferrie Road</p>
                <p className="text-sm text-gray-600">Melbourne, Australia</p>
                <p className="text-sm text-gray-600">Phone: 03-00000000</p>
                <p className="text-sm text-gray-600">Email: sales@awe.com</p>
              </div>
              <div className="text-right">
                <h3 className="mb-2">INVOICE</h3>
                <p className="text-sm"><span className="text-gray-600">Invoice #:</span> {order.invoiceNumber}</p>
                <p className="text-sm"><span className="text-gray-600">Date:</span> {new Date(order.createdAt).toLocaleDateString()}</p>
                <p className="text-sm"><span className="text-gray-600">Status:</span> <span className="px-2 py-1 bg-green-100 text-green-800 rounded text-xs">{order.status.toUpperCase()}</span></p>
              </div>
            </div>
          </div>

          {/* Bill To */}
          <div className="mb-6">
            <h4 className="mb-2">Bill To:</h4>
            <p className="text-sm">{order.shippingAddress.fullName}</p>
            <p className="text-sm text-gray-600">{order.shippingAddress.address}</p>
            <p className="text-sm text-gray-600">{order.shippingAddress.city}, {order.shippingAddress.postalCode}</p>
            <p className="text-sm text-gray-600">{order.shippingAddress.country}</p>
            <p className="text-sm text-gray-600">Phone: {order.shippingAddress.phone}</p>
          </div>

          {/* Items Table */}
          <div className="mb-6">
            <table className="w-full">
              <thead>
                <tr className="border-b-2 border-black">
                  <th className="text-left py-2 text-sm">Item</th>
                  <th className="text-right py-2 text-sm">Qty</th>
                  <th className="text-right py-2 text-sm">Price</th>
                  <th className="text-right py-2 text-sm">Subtotal</th>
                </tr>
              </thead>
              <tbody>
                {order.items.map((item, index) => (
                  <tr key={index} className="border-b border-gray-200">
                    <td className="py-3 text-sm">{item.productName}</td>
                    <td className="text-right py-3 text-sm">{item.quantity}</td>
                    <td className="text-right py-3 text-sm">{formatCurrency(item.price)}</td>
                    <td className="text-right py-3 text-sm">{formatCurrency(item.subtotal)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Totals */}
          <div className="flex justify-end">
            <div className="w-64 space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-gray-600">Subtotal:</span>
                <span>{formatCurrency(lineSubtotal)}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">VAT (10%):</span>
                <span>{formatCurrency(vat)}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Shipping:</span>
                <span>{formatCurrency(shipping)}</span>
              </div>
              <div className="flex justify-between border-t-2 border-black pt-2">
                <span>Total:</span>
                <span>{formatCurrency(order.total)}</span>
              </div>
            </div>
          </div>

          {/* Payment Info */}
          <div className="mt-8 pt-6 border-t border-gray-200">
            <p className="text-sm"><span className="text-gray-600">Payment Method:</span> {order.paymentMethod?.toUpperCase()}</p>
            {order.trackingNumber && (
              <p className="text-sm mt-1"><span className="text-gray-600">Tracking Number:</span> {order.trackingNumber}</p>
            )}
          </div>

          {/* Footer */}
          <div className="mt-8 pt-6 border-t border-gray-200 text-center text-xs text-gray-600">
            <p>Thank you for your business!</p>
            <p className="mt-1">For questions about this invoice, contact us at sales@awe.com</p>
          </div>
        </div>
      </div>
    </div>
  );
}
