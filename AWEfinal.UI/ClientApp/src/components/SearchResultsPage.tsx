import type { Product } from "../types";
import { ProductCard } from "./ProductCard";

interface SearchResultsPageProps {
  products: Product[];
  isLoading?: boolean;
  query: string;
  onProductClick?: (product: Product) => void;
  onAddToCart?: (product: Product) => void;
}

export function SearchResultsPage({
  products,
  isLoading,
  query,
  onProductClick,
  onAddToCart,
}: SearchResultsPageProps) {
  const trimmedQuery = query.trim();

  if (isLoading) {
    return (
      <div className="text-center text-sm text-gray-600 py-12">
        Searching products...
      </div>
    );
  }

  if (!products.length) {
    return (
      <div className="text-center text-sm text-gray-600 py-12 border-2 border-dashed border-gray-300 rounded">
        {trimmedQuery ? (
          <>
            No products found for{" "}
            "<span className="font-semibold">{trimmedQuery}</span>".
          </>
        ) : (
          "No products available."
        )}
      </div>
    );
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-xl">
          {trimmedQuery ? (
            <>
              Search results for{" "}
              <span className="font-semibold">"{trimmedQuery}"</span>
            </>
          ) : (
            "All products"
          )}
        </h2>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
        {products.map((product) => (
          <ProductCard
            key={product.id}
            product={product}
            onClick={() => onProductClick?.(product)}
            onAddToCart={onAddToCart}
          />
        ))}
      </div>
    </div>
  );
}
