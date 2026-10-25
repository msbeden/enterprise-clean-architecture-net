import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { Shield, Lock, Mail, ArrowRight, AlertCircle, Info } from 'lucide-react';
import { MainLayout } from '../../components/layout/MainLayout';

export const LoginPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const fromPath = (location.state as any)?.from?.pathname || '/admin';

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    const res = await login({ email, password });
    setLoading(false);

    if (res.success) {
      navigate(fromPath, { replace: true });
    } else {
      setError(res.message || 'Giriş başarısız oldu.');
    }
  };

  return (
    <MainLayout>
      <div style={{ maxWidth: '440px', margin: '40px auto' }}>
        <div className="glass-panel" style={{ padding: '40px 32px' }}>
          
          <div style={{ textAlign: 'center', marginBottom: '28px' }}>
            <div style={{ width: '56px', height: '56px', background: 'linear-gradient(135deg, #06b6d4, #6366f1)', borderRadius: '16px', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 16px', boxShadow: '0 0 20px rgba(6, 182, 212, 0.4)' }}>
              <Shield size={28} color="#ffffff" />
            </div>
            <h2 style={{ fontSize: '1.5rem', fontWeight: 800, color: '#f3f4f6', marginBottom: '6px' }}>Oturum Açın</h2>
            <p style={{ fontSize: '0.85rem', color: '#9ca3af' }}>Enterprise Commerce hesabınıza erişin</p>
          </div>

          {(location.state as any)?.from && (
            <div style={{ padding: '12px 16px', background: 'rgba(99,102,241,0.15)', border: '1px solid rgba(99,102,241,0.3)', borderRadius: '8px', color: '#a5b4fc', fontSize: '0.82rem', marginBottom: '20px', display: 'flex', alignItems: 'center', gap: '8px' }}>
              <Info size={18} />
              <span>Korumalı alana erişmek için lütfen önce giriş yapın.</span>
            </div>
          )}

          {error && (
            <div style={{ padding: '12px 16px', background: 'rgba(244,63,94,0.15)', border: '1px solid rgba(244,63,94,0.3)', borderRadius: '8px', color: '#fda4af', fontSize: '0.85rem', marginBottom: '20px', display: 'flex', alignItems: 'center', gap: '8px' }}>
              <AlertCircle size={18} />
              <span>{error}</span>
            </div>
          )}

          <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
            <div>
              <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '8px' }}>
                E-Posta Adresi
              </label>
              <div style={{ position: 'relative' }}>
                <Mail size={18} color="#6b7280" style={{ position: 'absolute', left: '14px', top: '50%', transform: 'translateY(-50%)' }} />
                <input
                  type="email"
                  required
                  className="form-control"
                  style={{ paddingLeft: '44px' }}
                  placeholder="ahmet.yilmaz@enterprise.com"
                  value={email}
                  onChange={e => setEmail(e.target.value)}
                />
              </div>
            </div>

            <div>
              <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '8px' }}>
                Şifre
              </label>
              <div style={{ position: 'relative' }}>
                <Lock size={18} color="#6b7280" style={{ position: 'absolute', left: '14px', top: '50%', transform: 'translateY(-50%)' }} />
                <input
                  type="password"
                  required
                  className="form-control"
                  style={{ paddingLeft: '44px' }}
                  placeholder="••••••••"
                  value={password}
                  onChange={e => setPassword(e.target.value)}
                />
              </div>
            </div>

            <button
              type="submit"
              disabled={loading}
              className="btn-primary"
              style={{ width: '100%', justifyContent: 'center', padding: '14px', marginTop: '8px' }}
            >
              {loading ? 'Giriş Yapılıyor...' : (
                <>
                  <span>Giriş Yap</span>
                  <ArrowRight size={18} />
                </>
              )}
            </button>
          </form>

        </div>
      </div>
    </MainLayout>
  );
};
