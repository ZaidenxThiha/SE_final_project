import { useState } from "react";
import { X } from "lucide-react";
import { login, register } from "../utils/auth";
import { User } from "../types";

interface LoginPageProps {
  onLogin: (user: User) => void;
  isModal?: boolean;
  onClose?: () => void;
}

export function LoginPage({ onLogin, isModal = false, onClose }: LoginPageProps) {
  const [isLogin, setIsLogin] = useState(true);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [name, setName] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    if (!isLogin && !name.trim()) {
      setError("Please enter your name");
      return;
    }

    setIsSubmitting(true);
    try {
      if (isLogin) {
        const user = await login(email, password);
        onLogin(user);
      } else {
        const user = await register(email, password, name);
        onLogin(user);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Authentication failed");
    } finally {
      setIsSubmitting(false);
    }
  };

  const card = (
    <div className="bg-white border-2 border-black rounded-lg p-8 w-full max-w-md relative">
      {isModal && onClose && (
        <button
          onClick={onClose}
          className="absolute right-4 top-4 rounded-full p-2 text-gray-500 hover:text-black hover:bg-gray-100 transition-colors"
          aria-label="Close"
        >
          <X className="w-4 h-4" />
        </button>
      )}
      <h2 className="text-center mb-6">{isLogin ? "Login" : "Create Account"}</h2>
        
      <form onSubmit={handleSubmit} className="space-y-4">
        {!isLogin && (
          <div>
            <label className="block text-sm mb-1">Full Name</label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="w-full border-2 border-black rounded px-3 py-2"
              required={!isLogin}
            />
          </div>
        )}
        
        <div>
          <label className="block text-sm mb-1">Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full border-2 border-black rounded px-3 py-2"
            required
          />
        </div>
        
        <div>
          <label className="block text-sm mb-1">Password</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full border-2 border-black rounded px-3 py-2"
            required
          />
        </div>

        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-3 py-2 rounded text-sm">
            {error}
          </div>
        )}

        <button
          type="submit"
          className="w-full py-3 rounded text-white disabled:opacity-60"
          style={{ backgroundColor: "#073634" }}
          disabled={isSubmitting}
        >
          {isSubmitting ? "Please wait..." : isLogin ? "Login" : "Create Account"}
        </button>
      </form>

      <div className="mt-6 text-center">
        <button
          onClick={() => {
            setIsLogin(!isLogin);
            setError("");
          }}
          className="text-sm hover:underline"
        >
          {isLogin ? "Don't have an account? Sign up" : "Already have an account? Login"}
        </button>
      </div>

      {isLogin && (
        <div className="mt-4 p-3 bg-gray-100 border border-gray-300 rounded text-xs">
          <p className="mb-1">Demo Credentials:</p>
          <p>Admin: admin@electrostore.com / admin123</p>
        </div>
      )}
    </div>
  );

  if (isModal) {
    return card;
  }

  return (
    <div className="min-h-screen flex items-center justify-center" style={{ backgroundColor: "#EDEECE" }}>
      {card}
    </div>
  );
}
