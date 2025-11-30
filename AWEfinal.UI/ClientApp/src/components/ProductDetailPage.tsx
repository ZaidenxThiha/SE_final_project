import { useMemo, useState } from "react";
import type { Product } from "../types";
import { formatCurrency } from "../utils/currency";

interface ProductDetailPageProps {
  product: Product;
  onBack?: () => void;
  onAddToCart?: (product: Product) => void;
  onBuyNow?: (product: Product) => void;
}

export function ProductDetailPage({
  product,
  onBack,
  onAddToCart,
  onBuyNow,
}: ProductDetailPageProps) {
  const [selectedSize, setSelectedSize] = useState(
    product.sizes?.[0] ?? "Standard"
  );
  const [selectedColor, setSelectedColor] = useState(product.colors?.[0] ?? "");
  const [selectedImageIndex, setSelectedImageIndex] = useState(0);

  const galleryImages = useMemo(() => {
    if (product.images?.length) {
      return product.images;
    }
    return ["/images/products/placeholder.jpg"];
  }, [product.images]);

  return (
    <div className="max-w-7xl mx-auto px-4 py-6">
      {/* Breadcrumb */}
      <div className="text-sm mb-6">
        <button onClick={onBack} className="hover:underline">Home</button>
        <span className="mx-1">›</span>
        <button onClick={onBack} className="hover:underline">Products</button>
        <span className="mx-1">›</span>
        <span>{product.name}</span>
      </div>

      <div className="flex gap-6">
        {/* Left Section - Product Images and Info */}
        <div className="flex-1 flex flex-col">
          {/* Main Product Image */}
          <div className="border-2 border-black rounded mb-4 bg-white flex items-center justify-center overflow-hidden w-full h-80">
            <img
              src={galleryImages[selectedImageIndex] || "/images/products/placeholder.jpg"}
              alt={product.name}
              className="h-full object-contain"
              onError={(e) => {
                (e.currentTarget as HTMLImageElement).src = "/images/products/placeholder.jpg";
              }}
            />
          </div>

          {/* Thumbnail Images */}
          <div className="flex gap-2 mb-6">
            {galleryImages.slice(0, 6).map((image, index) => (
              <button
                key={index}
                className="w-12 h-12 border-2 border-black rounded bg-white flex items-center justify-center cursor-pointer overflow-hidden"
                onClick={() => setSelectedImageIndex(index)}
              >
                <img
                  src={image}
                  alt={`${product.name} ${index + 1}`}
                  className="w-full h-full object-cover"
                  onError={(e) => {
                    (e.currentTarget as HTMLImageElement).src = "/images/products/placeholder.jpg";
                  }}
                />
              </button>
            ))}
          </div>

          {/* Product Information Section */}
          <div className="border-2 border-black rounded">
            <div className="border-b-2 border-black px-4 py-2">
              <h3>Product Information</h3>
            </div>
            <div className="p-4 h-48 overflow-y-auto text-sm">
              <div className="mb-4">
                <p className="mb-2">{product.description}</p>
              </div>
              <div className="mb-4">
                <p className="mb-1">Key Features:</p>
                <ul className="list-disc list-inside space-y-1">
                  {product.features.map((feature, index) => (
                    <li key={index}>{feature}</li>
                  ))}
                </ul>
              </div>
              <div>
                <p className="mb-1">Availability: {product.inStock ? "In Stock" : "Out of Stock"}</p>
                <p>Category: {product.category}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Right Section - Product Details */}
        <div className="w-80 flex-shrink-0">
          <div className="border-2 border-black rounded p-4" style={{ backgroundColor: '#EDEECE' }}>
            {/* Product Name and Price */}
            <h2 className="mb-1 text-2xl font-semibold">{product.name}</h2>
            <p className="mb-4 text-xl font-bold">{formatCurrency(product.price)}</p>

            {/* Size Dropdown */}
            {product.sizes && product.sizes.length > 0 && (
              <div className="mb-3">
                <label className="text-sm block mb-1">Size</label>
                <select 
                  value={selectedSize}
                  onChange={(e) => setSelectedSize(e.target.value)}
                  className="w-full border-2 border-black rounded px-2 py-1 text-sm"
                  style={{ backgroundColor: 'white' }}
                >
                  {product.sizes.map((size) => (
                    <option key={size} value={size}>{size}</option>
                  ))}
                </select>
              </div>
            )}

            {/* Color Dropdown */}
            <div className="mb-3">
              <label className="text-sm block mb-1">Color</label>
              <select 
                value={selectedColor}
                onChange={(e) => setSelectedColor(e.target.value)}
                className="w-full border-2 border-black rounded px-2 py-1 text-sm h-20"
                style={{ backgroundColor: 'white' }}
                size={Math.min(3, product.colors.length)}
              >
                {product.colors.map((color) => (
                  <option key={color} value={color}>{color}</option>
                ))}
              </select>
            </div>

            {/* Shipping Info */}
            <div className="border-2 border-black rounded px-3 py-2 mb-3 text-sm" style={{ backgroundColor: 'white' }}>
              <p>Transport: 4-5 days</p>
              <p>Free Shipping</p>
            </div>

            {/* Buttons */}
            <div className="flex gap-3">
              <button 
                onClick={() => onAddToCart?.(product)}
                className="flex-1 py-3 px-4 text-white text-base border-2 border-black rounded-full shadow-lg hover:shadow-xl transition-all duration-200"
                style={{ backgroundColor: '#073634' }}
              >
                Add to Cart
              </button>
              <button 
                onClick={() => onBuyNow?.(product)}
                className="flex-1 py-3 px-4 text-white text-base border-2 border-black rounded-full shadow-lg hover:shadow-xl transition-all duration-200"
                style={{ backgroundColor: '#DC2626' }}
              >
                Buy Now
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
