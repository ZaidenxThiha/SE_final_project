import { useState, useMemo } from "react";
import { ProductCard } from "./ProductCard";
import type { Product } from "../types";

interface ProductsPageProps {
  products: Product[];
  selectedCategory?: string | null;
  isLoading?: boolean;
  onProductClick?: (product: Product) => void;
  onAddToCart?: (product: Product) => void;
}

const PRODUCTS_PER_PAGE = 8;

export function ProductsPage({
  products,
  selectedCategory,
  isLoading,
  onProductClick,
  onAddToCart,
}: ProductsPageProps) {
  const [selectedFilter, setSelectedFilter] = useState("Price Increasing");
  const [currentPage, setCurrentPage] = useState(1);
  const [localCategory, setLocalCategory] = useState<string | null>(selectedCategory || null);
  
  const categories = [
    "All Categories",
    "Smartphones",
    "Laptops",
    "Tablets",
    "Headphones",
    "Smart TVs",
    "Cameras",
    "Smartwatches",
    "Gaming Consoles",
    "Speakers",
    "Monitors"
  ];
  
  const filterOptions = [
    "Price Increasing",
    "Price Decreasing", 
    "Name A - Z"
  ];

  // Filter and sort products
  const allFilteredProducts = useMemo(() => {
    const categoryToUse = localCategory || selectedCategory;
    let filtered = Array.isArray(products) ? [...products] : [];

    if (categoryToUse && categoryToUse !== "All Categories") {
      filtered = filtered.filter(
        (product) =>
          product.category?.toLowerCase() === categoryToUse.toLowerCase()
      );
    }

    const sorted = [...filtered];
    switch (selectedFilter) {
      case "Price Increasing":
        sorted.sort((a, b) => a.price - b.price);
        break;
      case "Price Decreasing":
        sorted.sort((a, b) => b.price - a.price);
        break;
      case "Name A - Z":
        sorted.sort((a, b) => a.name.localeCompare(b.name));
        break;
      default:
        break;
    }
    
    return sorted;
  }, [localCategory, selectedCategory, selectedFilter]);

  // Calculate pagination
  const totalPages = Math.ceil(allFilteredProducts.length / PRODUCTS_PER_PAGE);
  const startIndex = (currentPage - 1) * PRODUCTS_PER_PAGE;
  const endIndex = startIndex + PRODUCTS_PER_PAGE;
  const currentProducts = allFilteredProducts.slice(startIndex, endIndex);

  // Reset to page 1 when filter or category changes
  useMemo(() => {
    setCurrentPage(1);
  }, [localCategory, selectedCategory, selectedFilter]);

  return (
    <div className="max-w-7xl mx-auto px-4 py-6">
      <div className="flex gap-6">
        {/* Sidebar Filter */}
        <aside className="w-48 flex-shrink-0 space-y-4">
          {/* Categories Filter */}
          <div className="border-2 border-black rounded">
            <div className="border-b-2 border-black px-3 py-2 flex justify-between items-center">
              <span className="text-sm">Categories</span>
            </div>
            
            <div className="p-2">
              {categories.map((category) => {
                const isAllCategories = category === "All Categories";
                const currentCategory = localCategory || selectedCategory;
                const isSelected = (isAllCategories && !currentCategory) || currentCategory === category;
                
                return (
                  <button
                    key={category}
                    onClick={() => {
                      if (isAllCategories) {
                        setLocalCategory(null);
                      } else {
                        setLocalCategory(category);
                      }
                    }}
                    className={`w-full text-left text-sm px-2 py-1.5 border border-black mb-1 hover:bg-white transition-colors ${
                      isSelected ? "bg-white" : ""
                    }`}
                    style={{ 
                      backgroundColor: isSelected ? "white" : "#EDEECE"
                    }}
                  >
                    {category}
                  </button>
                );
              })}
            </div>
          </div>
          
          {/* Sort Filter */}
          <div className="border-2 border-black rounded">
            {/* Filter Header */}
            <div className="border-b-2 border-black px-3 py-2 flex justify-between items-center">
              <span className="text-sm">Sort By</span>
              <button className="text-sm">≡</button>
            </div>
            
            {/* Filter Options */}
            <div className="p-2">
              {filterOptions.map((option) => (
                <button
                  key={option}
                  onClick={() => setSelectedFilter(option)}
                  className={`w-full text-left text-sm px-2 py-1.5 border border-black mb-1 ${
                    selectedFilter === option ? "bg-white" : ""
                  }`}
                  style={{ 
                    backgroundColor: selectedFilter === option ? "white" : "#EDEECE"
                  }}
                >
                  {option}
                </button>
              ))}
            </div>
          </div>
        </aside>

        {/* Main Content */}
        <div className="flex-1">
          {isLoading ? (
            <div className="text-center text-sm text-gray-600 py-12">
              Loading products...
            </div>
          ) : currentProducts.length === 0 ? (
            <div className="text-center text-sm text-gray-600 py-12 border-2 border-dashed border-gray-300 rounded">
              {products.length === 0
                ? "No products available."
                : "No products match the selected filters."}
            </div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
              {currentProducts.map((product) => (
                <ProductCard
                  key={product.id}
                  product={product}
                  onClick={() => onProductClick?.(product)}
                  onAddToCart={onAddToCart}
                />
              ))}
            </div>
          )}

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex justify-between items-center">
              <div className="text-sm">
                Page {currentPage} of {totalPages} ({allFilteredProducts.length} products)
              </div>
              
              <div className="flex gap-2">
                <button 
                  onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                  disabled={currentPage === 1}
                  className="px-6 py-2 border-2 border-black rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-white transition-colors"
                  style={{ backgroundColor: '#EDEECE' }}
                >
                  Previous Page
                </button>
                
                <button 
                  onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
                  disabled={currentPage === totalPages}
                  className="px-6 py-2 border-2 border-black rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-white transition-colors"
                  style={{ backgroundColor: '#EDEECE' }}
                >
                  Next Page
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
