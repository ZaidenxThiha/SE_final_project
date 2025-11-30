import { useCallback, useEffect, useMemo, useState } from "react";
import { Plus, Edit2, Trash2, Save, X, Loader2 } from "lucide-react";
import type { Product } from "../types";
import { productsApi } from "../utils/api";
import { toast } from "sonner";
import { formatCurrency } from "../utils/currency";

type FormState = Partial<Product> & {
  colorsText?: string;
  featuresText?: string;
};

const defaultFormState: FormState = {
  name: "",
  price: 0,
  category: "Smartphones",
  description: "",
  storage: "",
  colors: [],
  features: [],
  images: [],
  sizes: [],
  inStock: true,
  stockQuantity: 0,
  rating: 5,
  colorsText: "",
  featuresText: "",
};

export function ProductManagement() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [isAdding, setIsAdding] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormState>(defaultFormState);
  const [uploadingImage, setUploadingImage] = useState(false);

  const fetchProducts = useCallback(async () => {
    setLoading(true);
    try {
      const data = await productsApi.list();
      setProducts(data);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to load products");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  const categories = useMemo(
    () => [
      "Smartphones",
      "Laptops",
      "Tablets",
      "Headphones",
      "Smart TVs",
      "Cameras",
      "Smartwatches",
      "Gaming Consoles",
      "Speakers",
      "Monitors",
    ],
    []
  );

  const resetForm = () => {
    setFormData(defaultFormState);
    setIsAdding(false);
    setEditingId(null);
  };

  const handleAdd = () => {
    setIsAdding(true);
    setEditingId(null);
    setFormData(defaultFormState);
  };

  const handleEdit = (product: Product) => {
    setEditingId(product.id);
    setIsAdding(false);
    setFormData({
      ...product,
      colorsText: product.colors?.join(", ") ?? "",
      featuresText: product.features?.join(", ") ?? "",
      images: product.images ?? [],
    });
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Delete this product?")) {
      return;
    }
    try {
      await productsApi.remove(id);
      setProducts((prev) => prev.filter((p) => p.id !== id));
      toast.success("Product deleted");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to delete product");
    }
  };

  const serializeForm = (): Partial<Product> => {
    const colors =
      formData.colorsText?.split(",").map((c) => c.trim()).filter(Boolean) ?? formData.colors ?? [];
    const features =
      formData.featuresText?.split(",").map((f) => f.trim()).filter(Boolean) ??
      formData.features ??
      [];

    return {
      ...formData,
      colors,
      features,
      images: formData.images ?? [],
      sizes: formData.sizes ?? [],
    };
  };

  const handleImageUpload = async (file: File) => {
    if (!editingId) {
      toast.error("Save the product before uploading images.");
      return;
    }
    try {
      setUploadingImage(true);
      const result = await productsApi.uploadImage(editingId, file);
      setFormData((prev) => ({ ...prev, images: result.images }));
      toast.success("Image uploaded");
      await fetchProducts();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to upload image");
    } finally {
      setUploadingImage(false);
    }
  };

  const handleSave = async () => {
    if (!formData.name || !formData.category || !formData.description) {
      toast.error("Name, category, and description are required.");
      return;
    }

    setIsSaving(true);
    try {
      const payload = serializeForm();
      if (isAdding) {
        await productsApi.create(payload);
        toast.success("Product created");
      } else if (editingId) {
        await productsApi.update(editingId, payload);
        toast.success("Product updated");
      }
      await fetchProducts();
      resetForm();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to save product");
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2>Product Management</h2>
        <button
          onClick={handleAdd}
          className="px-4 py-2 text-white rounded flex items-center gap-2 text-sm"
          style={{ backgroundColor: "#073634" }}
        >
          <Plus size={18} />
          Add New Product
        </button>
      </div>

      {(isAdding || editingId !== null) && (
        <div className="bg-white border-2 border-black rounded-lg p-6 mb-6">
          <h3 className="mb-4">{isAdding ? "Add New Product" : "Edit Product"}</h3>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm mb-1">Product Name</label>
              <input
                type="text"
                value={formData.name || ""}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                className="w-full border-2 border-black rounded px-3 py-2 text-sm"
              />
            </div>
            <div>
              <label className="block text-sm mb-1">Price (USD)</label>
              <input
                type="number"
                value={formData.price ?? 0}
                onChange={(e) => setFormData({ ...formData, price: Number(e.target.value) })}
                className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                min={0}
              />
            </div>
            <div>
              <label className="block text-sm mb-1">Stock Quantity</label>
              <input
                type="number"
                value={formData.stockQuantity ?? 0}
                onChange={(e) => setFormData({ ...formData, stockQuantity: Number(e.target.value) })}
                className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                min={0}
              />
            </div>
            <div>
              <label className="block text-sm mb-1">Category</label>
              <select
                value={formData.category || "Smartphones"}
                onChange={(e) => setFormData({ ...formData, category: e.target.value })}
                className="w-full border-2 border-black rounded px-3 py-2 text-sm"
              >
                {categories.map((category) => (
                  <option key={category}>{category}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm mb-1">Colors (comma-separated)</label>
              <input
                type="text"
                value={formData.colorsText ?? formData.colors?.join(", ") ?? ""}
                onChange={(e) => setFormData({ ...formData, colorsText: e.target.value })}
                className="w-full border-2 border-black rounded px-3 py-2 text-sm"
              />
            </div>
            <div>
              <label className="block text-sm mb-1">Features (comma-separated)</label>
              <input
                type="text"
                value={formData.featuresText ?? formData.features?.join(", ") ?? ""}
                onChange={(e) => setFormData({ ...formData, featuresText: e.target.value })}
                className="w-full border-2 border-black rounded px-3 py-2 text-sm"
              />
            </div>
            <div>
              <label className="block text-sm mb-1">Storage</label>
              <input
                type="text"
                value={formData.storage || ""}
                onChange={(e) => setFormData({ ...formData, storage: e.target.value })}
                className="w-full border-2 border-black rounded px-3 py-2 text-sm"
              />
            </div>
            <div>
              <label className="block text-sm mb-1">Rating (0-5)</label>
              <input
                type="number"
                value={formData.rating ?? 5}
                onChange={(e) => setFormData({ ...formData, rating: Number(e.target.value) })}
                className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                min={0}
                max={5}
                step={0.1}
              />
            </div>
            {editingId && (
              <div className="col-span-2">
                <label className="block text-sm mb-1">Product Images</label>
                <div className="space-y-2">
                  <input
                    type="file"
                    accept="image/*"
                    onChange={(e) => {
                      const file = e.target.files?.[0];
                      if (file) {
                        handleImageUpload(file);
                        e.target.value = "";
                      }
                    }}
                    disabled={uploadingImage}
                    className="w-full border-2 border-dashed border-black rounded px-3 py-2 text-sm bg-white"
                  />
                  <p className="text-xs text-gray-500">
                    Upload images after saving the product. One file per upload.
                  </p>
                  {formData.images && formData.images.length > 0 && (
                    <div className="flex flex-wrap gap-3">
                      {formData.images.map((image, index) => (
                        <div key={`${image}-${index}`} className="w-20 h-20 border border-black rounded overflow-hidden">
                          <img src={image} alt={`Product ${index + 1}`} className="w-full h-full object-cover" />
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
            )}
            {!editingId && (
              <div className="col-span-2 text-xs text-gray-500">
                Save the product first, then upload images.
              </div>
            )}
            <div className="col-span-2">
              <label className="block text-sm mb-1">Description</label>
              <textarea
                value={formData.description || ""}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                rows={3}
              />
            </div>
            <div>
              <label className="flex items-center gap-2 text-sm">
                <input
                  type="checkbox"
                  checked={formData.inStock ?? true}
                  onChange={(e) => setFormData({ ...formData, inStock: e.target.checked })}
                />
                In Stock
              </label>
            </div>
          </div>

          <div className="flex gap-2 mt-4">
            <button
              onClick={handleSave}
              className="px-4 py-2 text-white rounded flex items-center gap-2 text-sm disabled:opacity-60"
              style={{ backgroundColor: "#073634" }}
              disabled={isSaving}
            >
              {isSaving ? <Loader2 size={16} className="animate-spin" /> : <Save size={18} />}
              Save
            </button>
            <button
              onClick={resetForm}
              className="px-4 py-2 border-2 border-black rounded flex items-center gap-2 text-sm"
              style={{ backgroundColor: "#EDEECE" }}
            >
              <X size={18} />
              Cancel
            </button>
          </div>
        </div>
      )}

      <div className="bg-white border-2 border-black rounded-lg overflow-hidden">
        <table className="w-full">
          <thead className="border-b-2 border-black" style={{ backgroundColor: "#EDEECE" }}>
            <tr>
              <th className="px-4 py-3 text-left text-sm">ID</th>
              <th className="px-4 py-3 text-left text-sm">Name</th>
              <th className="px-4 py-3 text-left text-sm">Category</th>
              <th className="px-4 py-3 text-left text-sm">Price</th>
              <th className="px-4 py-3 text-left text-sm">Stock</th>
              <th className="px-4 py-3 text-left text-sm">Actions</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={6} className="px-4 py-6 text-center text-sm text-gray-500">
                  Loading products...
                </td>
              </tr>
            ) : products.length === 0 ? (
              <tr>
                <td colSpan={6} className="px-4 py-6 text-center text-sm text-gray-500">
                  No products found.
                </td>
              </tr>
            ) : (
              products.map((product, index) => (
                <tr key={product.id} className={index % 2 === 0 ? "bg-white" : "bg-gray-50"}>
                  <td className="px-4 py-3 text-sm">{product.id}</td>
                  <td className="px-4 py-3 text-sm">{product.name}</td>
                  <td className="px-4 py-3 text-sm">{product.category}</td>
                  <td className="px-4 py-3 text-sm">{formatCurrency(product.price)}</td>
                  <td className="px-4 py-3 text-sm">
                    <span
                      className={`px-2 py-1 rounded text-xs ${
                        product.inStock ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"
                      }`}
                    >
                      {product.inStock ? `In Stock (${product.stockQuantity})` : "Out of Stock"}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-sm">
                    <div className="flex gap-2">
                      <button onClick={() => handleEdit(product)} className="p-1 hover:bg-gray-200 rounded">
                        <Edit2 size={16} />
                      </button>
                      <button
                        onClick={() => handleDelete(product.id)}
                        className="p-1 hover:bg-red-100 text-red-600 rounded"
                      >
                        <Trash2 size={16} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
