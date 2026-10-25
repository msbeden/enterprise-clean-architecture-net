import React, { useState, useEffect } from 'react';
import { AdminLayout } from '../../components/layout/AdminLayout';
import type { UserDto } from '../../types/user.types';
import { apiClient } from '../../services/api';
import { Plus, Users, X, CheckCircle, ShieldCheck } from 'lucide-react';

export const AdminUsersPage: React.FC = () => {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);

  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [role, setRole] = useState('User');
  const [formLoading, setFormLoading] = useState(false);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  const loadData = () => {
    setLoading(true);
    apiClient.getUsers().then(res => {
      setLoading(false);
      if (res.isSuccess && res.value) {
        setUsers(res.value);
      }
    });
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleCreateUser = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormLoading(true);
    setSuccessMsg(null);

    const res = await apiClient.createUser({
      firstName,
      lastName,
      email,
      password,
      role
    });

    setFormLoading(false);
    if (res.isSuccess) {
      setSuccessMsg('Kullanıcı başarıyla oluşturuldu!');
      loadData();
      setTimeout(() => {
        setShowModal(false);
        setSuccessMsg(null);
        setFirstName('');
        setLastName('');
        setEmail('');
        setPassword('');
      }, 1000);
    }
  };

  return (
    <AdminLayout>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '28px' }}>
        <div>
          <h1 style={{ fontSize: '1.8rem', fontWeight: 800, color: '#f3f4f6', marginBottom: '4px' }}>Üye Yönetimi</h1>
          <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>Sisteme kayıtlı yetkili kullanıcılar ve yeni üye tanımlama</p>
        </div>

        <button onClick={() => setShowModal(true)} className="btn-primary">
          <Plus size={18} />
          <span>Yeni Üye Tanımla</span>
        </button>
      </div>

      <div className="glass-panel" style={{ overflow: 'hidden' }}>
        {loading ? (
          <div style={{ padding: '40px', textAlign: 'center', color: '#9ca3af' }}>Kullanıcılar yükleniyor...</div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Ad Soyad</th>
                <th>E-Posta</th>
                <th>Rol</th>
                <th>Kayıt Tarihi</th>
              </tr>
            </thead>
            <tbody>
              {users.map(usr => (
                <tr key={usr.id}>
                  <td>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                      <div style={{ width: '36px', height: '36px', background: 'rgba(99,102,241,0.2)', border: '1px solid rgba(99,102,241,0.4)', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#a5b4fc', fontWeight: 700 }}>
                        {usr.firstName[0]}
                      </div>
                      <div>
                        <strong style={{ color: '#f3f4f6' }}>{usr.firstName} {usr.lastName}</strong>
                        <div style={{ fontSize: '0.75rem', color: '#6b7280' }}>ID: {usr.id}</div>
                      </div>
                    </div>
                  </td>
                  <td style={{ color: '#06b6d4' }}>{usr.email}</td>
                  <td>
                    <span className={usr.role === 'Admin' ? 'badge badge-indigo' : 'badge badge-cyan'}>
                      <ShieldCheck size={12} />
                      {usr.role}
                    </span>
                  </td>
                  <td style={{ color: '#9ca3af' }}>{usr.createdAt}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {showModal && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 50, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '20px' }}>
          <div className="glass-panel" style={{ width: '100%', maxWidth: '500px', padding: '32px', background: '#0f172a' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
              <h3 style={{ fontSize: '1.3rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: '10px' }}>
                <Users color="#f59e0b" /> Yeni Üye Tanımla
              </h3>
              <button onClick={() => setShowModal(false)} style={{ background: 'none', border: 'none', color: '#9ca3af', cursor: 'pointer' }}>
                <X size={24} />
              </button>
            </div>

            {successMsg && (
              <div style={{ padding: '12px 16px', background: 'rgba(16,185,129,0.15)', border: '1px solid rgba(16,185,129,0.3)', borderRadius: '8px', color: '#6ee7b7', fontSize: '0.85rem', marginBottom: '20px', display: 'flex', alignItems: 'center', gap: '8px' }}>
                <CheckCircle size={18} /> {successMsg}
              </div>
            )}

            <form onSubmit={handleCreateUser} style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                <div>
                  <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Ad</label>
                  <input required type="text" className="form-control" placeholder="Örn: Mehmet" value={firstName} onChange={e => setFirstName(e.target.value)} />
                </div>
                <div>
                  <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Soyad</label>
                  <input required type="text" className="form-control" placeholder="Örn: Demir" value={lastName} onChange={e => setLastName(e.target.value)} />
                </div>
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>E-Posta Adresi</label>
                <input required type="email" className="form-control" placeholder="mehmet.demir@enterprise.com" value={email} onChange={e => setEmail(e.target.value)} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Şifre</label>
                <input required type="password" className="form-control" placeholder="••••••••" value={password} onChange={e => setPassword(e.target.value)} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Kullanıcı Rolü</label>
                <select className="form-control" value={role} onChange={e => setRole(e.target.value)}>
                  <option value="User" style={{ background: '#0f172a' }}>Standart Kullanıcı</option>
                  <option value="Manager" style={{ background: '#0f172a' }}>Sistem Yöneticisi (Manager)</option>
                  <option value="Admin" style={{ background: '#0f172a' }}>Sistem Yöneticisi (Admin)</option>
                </select>
              </div>

              <button type="submit" disabled={formLoading} className="btn-primary" style={{ width: '100%', justifyContent: 'center', padding: '12px', marginTop: '8px' }}>
                {formLoading ? 'Oluşturuluyor...' : 'Kullanıcıyı Kaydet'}
              </button>
            </form>

          </div>
        </div>
      )}
    </AdminLayout>
  );
};
