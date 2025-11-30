import { ShoppingCart } from "lucide-react";
import type { Product } from "../types";
import { formatCurrency } from "../utils/currency";

interface ProductCardProps {
  product: Product;
  onClick?: () => void;
  onAddToCart?: (product: Product) => void;
}

export function ProductCard({ product, onClick, onAddToCart }: ProductCardProps) {
  const primaryImage = product.images?.[0] || "/images/products/placeholder.jpg";

  return (
    <div
      className="rounded-xl overflow-hidden border-2 border-black p-3 cursor-pointer hover:shadow-lg transition-shadow"
      style={{ backgroundColor: "#EDEECE" }}
      onClick={onClick}
    >
      <div className="mb-3 h-36 border border-black rounded-lg overflow-hidden bg-white flex items-center justify-center">
        <img
          src={primaryImage}
          alt={product.name}
          className="w-full h-full object-cover"
          onError={(e) => {
            (e.currentTarget as HTMLImageElement).src = "/images/products/placeholder.jpg";
          }}
        />
      </div>

      <div className="space-y-1">
        <div className="flex items-start justify-between">
          <div>
            <div className="text-sm">{product.name}</div>
            <div className="text-xs text-gray-600">{product.storage || product.category}</div>
          </div>
          <button
            onClick={(e) => {
              e.stopPropagation();
              onAddToCart?.(product);
            }}
            className="p-1.5 rounded-full border border-black hover:bg-black hover:text-white transition-colors"
          >
            <ShoppingCart className="w-4 h-4" />
          </button>
        </div>

        <div className="text-sm">{formatCurrency(product.price)}</div>

        <div className="text-xs text-gray-700 line-clamp-2">{product.description}</div>
      </div>
    </div>
  );
}
