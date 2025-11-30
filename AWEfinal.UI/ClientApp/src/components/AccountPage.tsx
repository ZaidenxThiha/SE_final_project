import { useEffect, useState } from "react";
import { createPortal } from "react-dom";
import {
  UserCircle,
  Mail,
  Calendar,
  Shield,
  Home,
  History,
  X as CloseIcon,
} from "lucide-react";
import type { User, UserProfileResponse } from "../types";
import { profileApi } from "../utils/api";
import { formatCurrency } from "../utils/currency";
import { toast } from "sonner";

interface AccountPageProps {
  user: User;
  onLogout: () => void;
  onViewOrders: () => void;
  onBackToHome: () => void;
  onUserUpdate?: (updated: User) => void;
}

type ActiveModal = "profile" | "address" | "password" | null;

export function AccountPage({
  user,
  onLogout,
  onViewOrders,
  onBackToHome,
  onUserUpdate,
}: AccountPageProps) {
  const [profile, setProfile] = useState<UserProfileResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [activeModal, setActiveModal] = useState<ActiveModal>(null);
  const [formState, setFormState] = useState({
    name: user.name,
    phone: user.phone ?? "",
    addressLine1: user.addressLine1 ?? "",
    addressLine2: user.addressLine2 ?? "",
    city: user.city ?? "",
    postalCode: user.postalCode ?? "",
    country: user.country ?? "",
  });
  const [passwordForm, setPasswordForm] = useState({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
  });
  const [changingPassword, setChangingPassword] = useState(false);

  useEffect(() => {
    const loadProfile = async () => {
      setLoading(true);
      try {
        const data = await profileApi.get();
        setProfile(data);
        setFormState({
          name: data.user.name,
          phone: data.user.phone ?? "",
          addressLine1: data.user.addressLine1 ?? "",
          addressLine2: data.user.addressLine2 ?? "",
          city: data.user.city ?? "",
          postalCode: data.user.postalCode ?? "",
          country: data.user.country ?? "",
        });
      } catch (error) {
        toast.error(error instanceof Error ? error.message : "Failed to load profile");
      } finally {
        setLoading(false);
      }
    };

    loadProfile();
  }, [user.id]);

  const profileUser = profile?.user ?? user;
  const stats = profile?.stats;

  const handleInputChange = (field: keyof typeof formState, value: string) => {
    setFormState((prev) => ({ ...prev, [field]: value }));
  };

  const resetFormState = () => {
    setFormState({
      name: profileUser.name,
      phone: profileUser.phone ?? "",
      addressLine1: profileUser.addressLine1 ?? "",
      addressLine2: profileUser.addressLine2 ?? "",
      city: profileUser.city ?? "",
      postalCode: profileUser.postalCode ?? "",
      country: profileUser.country ?? "",
    });
  };

  const handleSaveProfile = async () => {
    setSaving(true);
    try {
      const updated = await profileApi.update(formState);
      setProfile((prev) => (prev ? { ...prev, user: updated } : prev));
      onUserUpdate?.(updated);
      toast.success("Profile updated");
      setActiveModal(null);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to update profile");
    } finally {
      setSaving(false);
    }
  };

  const handleSaveAddress = async () => {
    setSaving(true);
    try {
      const updated = await profileApi.update(formState);
      setProfile((prev) => (prev ? { ...prev, user: updated } : prev));
      onUserUpdate?.(updated);
      toast.success("Address updated");
      setActiveModal(null);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to update address");
    } finally {
      setSaving(false);
    }
  };

  const handleChangePassword = async () => {
    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      toast.error("New password and confirmation do not match.");
      return;
    }

    setChangingPassword(true);
    try {
      await profileApi.changePassword(passwordForm.currentPassword, passwordForm.newPassword);
      toast.success("Password updated");
      setPasswordForm({ currentPassword: "", newPassword: "", confirmPassword: "" });
      setActiveModal(null);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to change password");
    } finally {
      setChangingPassword(false);
    }
  };

  const renderModal = () => {
    if (!activeModal || typeof document === "undefined") return null;

    return createPortal(
      <div className="auth-overlay" onClick={() => setActiveModal(null)}>
        <div
          className="auth-modal bg-white border-2 border-black rounded-lg p-6 max-h-[90vh] overflow-y-auto relative"
          onClick={(e) => e.stopPropagation()}
        >
          <button
            onClick={() => setActiveModal(null)}
            className="absolute right-4 top-4 p-2 border border-black rounded-full hover:bg-gray-100"
            aria-label="Close"
          >
            <CloseIcon size={14} />
          </button>
          {activeModal === "profile" && (
            <>
              <h3 className="mb-4 text-lg">Edit Profile</h3>
              <div className="space-y-3">
                <div>
                  <label className="block text-sm mb-1">Full Name</label>
                  <input
                    type="text"
                    value={formState.name}
                    onChange={(e) => handleInputChange("name", e.target.value)}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                  />
                </div>
                <div>
                  <label className="block text-sm mb-1">Phone</label>
                  <input
                    type="tel"
                    value={formState.phone}
                    onChange={(e) => handleInputChange("phone", e.target.value)}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                  />
                </div>
                <div>
                  <label className="block text-sm mb-1">Address Line 1</label>
                  <input
                    type="text"
                    value={formState.addressLine1}
                    onChange={(e) => handleInputChange("addressLine1", e.target.value)}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                  />
                </div>
                <div>
                  <label className="block text-sm mb-1">Address Line 2</label>
                  <input
                    type="text"
                    value={formState.addressLine2}
                    onChange={(e) => handleInputChange("addressLine2", e.target.value)}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                  />
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <label className="block text-sm mb-1">City</label>
                    <input
                      type="text"
                      value={formState.city}
                      onChange={(e) => handleInputChange("city", e.target.value)}
                      className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-sm mb-1">Postal Code</label>
                    <input
                      type="text"
                      value={formState.postalCode}
                      onChange={(e) => handleInputChange("postalCode", e.target.value)}
                      className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                    />
                  </div>
                </div>
                <div>
                  <label className="block text-sm mb-1">Country</label>
                  <input
                    type="text"
                    value={formState.country}
                    onChange={(e) => handleInputChange("country", e.target.value)}
                    className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                  />
                </div>
              </div>
              <div className="flex gap-2 mt-6">
                <button
                  onClick={handleSaveProfile}
                  className="px-4 py-2 border-2 border-black rounded text-sm text-white disabled:opacity-60"
                  style={{ backgroundColor: "#073634" }}
                  disabled={saving}
                >
                  {saving ? "Saving..." : "Save"}
                </button>
                <button
                  onClick={() => {
                    resetFormState();
                    setActiveModal(null);
                  }}
                  className="px-4 py-2 border-2 border-black rounded text-sm hover:bg-gray-100"
                  style={{ backgroundColor: "#EDEECE" }}
                >
                  Cancel
                </button>
              </div>
            </>
          )}

          {activeModal === "address" && (
            <>
              <h3 className="mb-4 text-lg">Edit Address</h3>
              <div className="space-y-3">
                {[
                  { key: "addressLine1", label: "Address Line 1" },
                  { key: "addressLine2", label: "Address Line 2" },
                  { key: "city", label: "City" },
                  { key: "postalCode", label: "Postal Code" },
                  { key: "country", label: "Country" },
                ].map(({ key, label }) => (
                  <div key={key}>
                    <label className="block text-sm mb-1">{label}</label>
                    <input
                      type="text"
                      value={(formState as any)[key]}
                      onChange={(e) => handleInputChange(key as keyof typeof formState, e.target.value)}
                      className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                    />
                  </div>
                ))}
              </div>
              <div className="flex gap-2 mt-6">
                <button
                  onClick={handleSaveAddress}
                  className="px-4 py-2 border-2 border-black rounded text-sm text-white disabled:opacity-60"
                  style={{ backgroundColor: "#073634" }}
                  disabled={saving}
                >
                  {saving ? "Saving..." : "Save"}
                </button>
                <button
                  onClick={() => {
                    resetFormState();
                    setActiveModal(null);
                  }}
                  className="px-4 py-2 border-2 border-black rounded text-sm hover:bg-gray-100"
                  style={{ backgroundColor: "#EDEECE" }}
                >
                  Cancel
                </button>
              </div>
            </>
          )}

          {activeModal === "password" && (
            <>
              <h3 className="mb-4 text-lg">Change Password</h3>
              <div className="space-y-3">
                <input
                  type="password"
                  placeholder="Current password"
                  value={passwordForm.currentPassword}
                  onChange={(e) =>
                    setPasswordForm((prev) => ({ ...prev, currentPassword: e.target.value }))
                  }
                  className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                />
                <input
                  type="password"
                  placeholder="New password"
                  value={passwordForm.newPassword}
                  onChange={(e) =>
                    setPasswordForm((prev) => ({ ...prev, newPassword: e.target.value }))
                  }
                  className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                />
                <input
                  type="password"
                  placeholder="Confirm new password"
                  value={passwordForm.confirmPassword}
                  onChange={(e) =>
                    setPasswordForm((prev) => ({ ...prev, confirmPassword: e.target.value }))
                  }
                  className="w-full border-2 border-black rounded px-3 py-2 text-sm"
                />
              </div>
              <div className="flex gap-2 mt-6">
                <button
                  onClick={handleChangePassword}
                  className="px-4 py-2 border-2 border-black rounded text-sm text-white disabled:opacity-60"
                  style={{ backgroundColor: "#073634" }}
                  disabled={changingPassword}
                >
                  {changingPassword ? "Updating..." : "Update Password"}
                </button>
                <button
                  onClick={() => setActiveModal(null)}
                  className="px-4 py-2 border-2 border-black rounded text-sm hover:bg-gray-100"
                  style={{ backgroundColor: "#EDEECE" }}
                >
                  Cancel
                </button>
              </div>
            </>
          )}
        </div>
      </div>,
      document.body
    );
  };

  return (
    <div className="min-h-screen" style={{ backgroundColor: "#EDEECE" }}>
      <div className="max-w-4xl mx-auto px-6 py-6">
        <div className="flex justify-between items-center mb-6">
          <h1>My Account</h1>
          <button
            onClick={onBackToHome}
            className="p-2 border-2 border-black rounded-full hover:bg-white transition-colors"
            style={{ backgroundColor: "#EDEECE" }}
            aria-label="Back to home"
          >
            <Home size={18} />
          </button>
        </div>

        <div className="grid grid-cols-3 gap-6">
          <div className="col-span-2 bg-white border-2 border-black rounded-lg p-6">
            <h3 className="mb-4">Profile Information</h3>
            {loading ? (
              <p className="text-sm text-gray-500">Loading profile...</p>
            ) : (
              <>
                <div className="space-y-4 mb-6">
                  <div className="flex items-center gap-3">
                    <UserCircle size={20} className="text-gray-600" />
                    <div>
                      <p className="text-xs text-gray-600">Full Name</p>
                      <p className="text-sm">{profileUser.name}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <Mail size={20} className="text-gray-600" />
                    <div>
                      <p className="text-xs text-gray-600">Email Address</p>
                      <p className="text-sm">{profileUser.email}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <Shield size={20} className="text-gray-600" />
                    <div>
                      <p className="text-xs text-gray-600">Account Type</p>
                      <p className="text-sm capitalize">{profileUser.role}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <Calendar size={20} className="text-gray-600" />
                    <div>
                      <p className="text-xs text-gray-600">Member Since</p>
                      <p className="text-sm">{new Date(profileUser.createdAt).toLocaleDateString()}</p>
                    </div>
                  </div>
                </div>

                <div className="text-sm space-y-1">
                  <p className="text-gray-600">Phone: {profileUser.phone || "—"}</p>
                  <p className="text-gray-600">
                    Address: {profileUser.addressLine1 || "—"}
                    {profileUser.addressLine2 ? `, ${profileUser.addressLine2}` : ""}
                  </p>
                  <p className="text-gray-600">
                    {profileUser.city || "—"} {profileUser.postalCode || ""} {profileUser.country || ""}
                  </p>
                </div>

                <div className="mt-6 pt-6 border-t border-gray-200">
                  <button
                    onClick={() => setActiveModal("profile")}
                    className="px-4 py-2 border-2 border-black rounded text-sm hover:bg-gray-100"
                    style={{ backgroundColor: "#EDEECE" }}
                  >
                    Edit Profile
                  </button>
                </div>
              </>
            )}
          </div>

          <div className="space-y-4">
            <div className="bg-white border-2 border-black rounded-lg p-6">
              <h4 className="mb-3">Quick Actions</h4>
              <div className="space-y-2">
                <button
                  onClick={onViewOrders}
                  className="w-full px-4 py-3 border-2 border-black rounded hover:bg-gray-100 flex items-center justify-center gap-2"
                  style={{ backgroundColor: "#EDEECE" }}
                  aria-label="View orders"
                >
                  <History size={18} />
                  <span>View Order History</span>
                </button>
              </div>
            </div>

            <div className="bg-white border-2 border-black rounded-lg p-6">
              <h4 className="mb-3">Security</h4>
              <div className="space-y-2">
                <button
                  onClick={() => setActiveModal("password")}
                  className="w-full px-4 py-3 border-2 border-black rounded hover:bg-gray-100 flex items-center justify-center gap-2"
                  style={{ backgroundColor: "#EDEECE" }}
                  aria-label="Change password"
                >
                  <Shield size={18} />
                  <span>Change Password</span>
                </button>
                <button
                  onClick={onLogout}
                  className="w-full px-4 py-2 bg-red-500 text-white rounded text-sm hover:bg-red-600"
                >
                  Logout
                </button>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white border-2 border-black rounded-lg p-6 mt-6">
          <div className="flex justify-between items-center mb-4">
            <h3>Account Statistics</h3>
            <button
              onClick={() => {
                setLoading(true);
                profileApi
                  .get()
                  .then((data) => {
                    setProfile(data);
                    toast.success("Statistics refreshed");
                  })
                  .catch((error) => toast.error(error instanceof Error ? error.message : "Failed to refresh"))
                  .finally(() => setLoading(false));
              }}
              className="text-xs underline"
            >
              Refresh
            </button>
          </div>

          {stats ? (
            <div className="grid grid-cols-2 gap-6">
              <div className="text-center">
                <p className="text-2xl mb-1">{stats.totalOrders}</p>
                <p className="text-sm text-gray-600">Total Orders</p>
              </div>
              <div className="text-center">
                <p className="text-2xl mb-1">{stats.deliveredOrders}</p>
                <p className="text-sm text-gray-600">Delivered</p>
              </div>
              <div className="text-center">
                <p className="text-2xl mb-1">{stats.inProgressOrders}</p>
                <p className="text-sm text-gray-600">In Progress</p>
              </div>
              <div className="text-center">
                <p className="text-2xl mb-1">{formatCurrency(stats.totalSpent)}</p>
                <p className="text-sm text-gray-600">Total Spent</p>
              </div>
            </div>
          ) : (
            <p className="text-sm text-gray-500">No stats available yet.</p>
          )}
        </div>
      </div>
      {renderModal()}
    </div>
  );
}
