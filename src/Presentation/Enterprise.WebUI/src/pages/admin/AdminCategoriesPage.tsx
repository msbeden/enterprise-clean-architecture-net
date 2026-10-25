import React, { useState, useEffect } from 'react';
import { AdminLayout } from '../../components/layout/AdminLayout';
import type { CategoryDto } from '../../types/category.types';
import { apiClient } from '../../services/api';
import { Plus, Grid, X, CheckCircle, Edit, Trash2, AlertTriangle } from 'lucide-react';

export const AdminCategoriesPage: React.FC = () => {
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingCategory, setEditingCategory] = useState<CategoryDto | null>(null);

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [formLoading, setFormLoading] = useState(false);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  const [deletingCategory, setDeletingCategory] = useState<CategoryDto | null>(null);
  const [deleteLoading, setDeleteLoading] = useState(false);

  const loadData = () => {
    setLoading(true);
    apiClient.getCategories().then(res => {
      setLoading(false);
      if (res.isSuccess && res.value) {
        setCategories(res.value);
      }
    });
  };

  useEffect(() => {
    loadData();
  }, []);

  const openCreateModal = () => {
    setEditingCategory(null);
    setName('');
    setDescription('');
    setSuccessMsg(null);
    setShowModal(true);
  };

  const openEditModal = (cat: CategoryDto) => {
    setEditingCategory(cat);
    setName(cat.name);
    setDescription(cat.description);
    setSuccessMsg(null);
    setShowModal(true);
  };

  const handleSaveCategory = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormLoading(true);
    setSuccessMsg(null);

    let res;
    if (editingCategory) {
      res = await apiClient.updateCategory({ id: editingCategory.id, name, description });
    } else {
      res = await apiClient.createCategory({ name, description });
    }

    setFormLoading(false);
    if (res.isSuccess) {
      setSuccessMsg(editingCategory ? 'Kategori başarıyla güncellendi!' : 'Kategori başarıyla oluşturuldu!');
      loadData();
      setTimeout(() => {
        setShowModal(false);
        setSuccessMsg(null);
      }, 1000);
    }
  };

  const handleDelete = async () => {
    if (!deletingCategory) return;
    setDeleteLoading(true);
    const res = await apiClient.deleteCategory(deletingCategory.id);
    setDeleteLoading(false);
    if (res.isSuccess) {
      setDeletingCategory(null);
      loadData();
    }
  };

  return (
    <AdminLayout>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '28px' }}>
        <div>
          <h1 style={{ fontSize: '1.8rem', fontWeight: 800, color: '#f3f4f6', marginBottom: '4px' }}>Kategori Yönetimi</h1>
          <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>Ürünlerin gruplandığı aktif kategoriler, yeni kategori ekleme, düzenleme ve silme</p>
        </div>

        <button onClick={openCreateModal} className="btn-primary">
          <Plus size={18} />
          <span>Yeni Kategori Ekle</span>
        </button>
      </div>

      <div className="glass-panel" style={{ overflow: 'hidden' }}>
        {loading ? (
          <div style={{ padding: '40px', textAlign: 'center', color: '#9ca3af' }}>Kategoriler yükleniyor...</div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Kategori Adı</th>
                <th>Açıklama</th>
                <th>Toplam Ürün Sayısı</th>
                <th style={{ textAlign: 'right' }}>İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {categories.map(cat => (
                <tr key={cat.id}>
                  <td>
                    <span className="badge badge-indigo">{cat.id}</span>
                  </td>
                  <td>
                    <strong style={{ color: '#f3f4f6' }}>{cat.name}</strong>
                  </td>
                  <td style={{ color: '#9ca3af' }}>{cat.description}</td>
                  <td>
                    <span className="badge badge-cyan">{cat.productCount || 0} Ürün</span>
                  </td>
                  <td style={{ textAlign: 'right' }}>
                    <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '8px' }}>
                      <button onClick={() => openEditModal(cat)} style={{ background: 'rgba(99,102,241,0.15)', border: '1px solid rgba(99,102,241,0.3)', color: '#818cf8', padding: '6px 10px', borderRadius: '6px', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '4px', fontSize: '0.8rem', fontWeight: 600 }}>
                        <Edit size={14} /> Düzenle
                      </button>
                      <button onClick={() => setDeletingCategory(cat)} style={{ background: 'rgba(239,68,68,0.15)', border: '1px solid rgba(239,68,68,0.3)', color: '#f87171', padding: '6px 10px', borderRadius: '6px', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '4px', fontSize: '0.8rem', fontWeight: 600 }}>
                        <Trash2 size={14} /> Sil
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* Create / Edit Modal */}
      {showModal && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 50, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '20px' }}>
          <div className="glass-panel" style={{ width: '100%', maxWidth: '480px', padding: '32px', background: '#0f172a' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
              <h3 style={{ fontSize: '1.3rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: '10px' }}>
                <Grid color="#6366f1" /> {editingCategory ? 'Kategoriyi Düzenle' : 'Yeni Kategori Ekle'}
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

            <form onSubmit={handleSaveCategory} style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <div>
                <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Kategori Adı</label>
                <input required type="text" className="form-control" placeholder="Örn: Sunucu & Depolama" value={name} onChange={e => setName(e.target.value)} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Kategori Açıklaması</label>
                <textarea required className="form-control" rows={3} placeholder="Kategori kapsamını açıklayın..." value={description} onChange={e => setDescription(e.target.value)} />
              </div>

              <button type="submit" disabled={formLoading} className="btn-primary" style={{ width: '100%', justifyContent: 'center', padding: '12px', marginTop: '8px' }}>
                {formLoading ? 'Kaydediliyor...' : editingCategory ? 'Değişiklikleri Kaydet' : 'Kategoriyi Oluştur'}
              </button>
            </form>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {deletingCategory && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 50, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '20px' }}>
          <div className="glass-panel" style={{ width: '100%', maxWidth: '420px', padding: '28px', background: '#0f172a', textAlign: 'center' }}>
            <div style={{ display: 'inline-flex', padding: '16px', background: 'rgba(239,68,68,0.15)', borderRadius: '50%', color: '#ef4444', marginBottom: '16px' }}>
              <AlertTriangle size={32} />
            </div>
            <h3 style={{ fontSize: '1.2rem', fontWeight: 700, marginBottom: '8px' }}>Kategoriyi Silmek İstediğinize Emin Misiniz?</h3>
            <p style={{ color: '#9ca3af', fontSize: '0.88rem', marginBottom: '24px' }}>
              <strong style={{ color: '#f3f4f6' }}>"{deletingCategory.name}"</strong> kategorisini sistemden silmek üzeresiniz.
            </p>
            <div style={{ display: 'flex', gap: '12px', justifyContent: 'center' }}>
              <button onClick={() => setDeletingCategory(null)} style={{ padding: '10px 20px', borderRadius: '8px', border: '1px solid rgba(255,255,255,0.1)', background: 'transparent', color: '#9ca3af', cursor: 'pointer', fontWeight: 600 }}>
                İptal
              </button>
              <button onClick={handleDelete} disabled={deleteLoading} style={{ padding: '10px 20px', borderRadius: '8px', border: 'none', background: '#ef4444', color: '#ffffff', cursor: 'pointer', fontWeight: 600 }}>
                {deleteLoading ? 'Siliniyor...' : 'Evet, Sil'}
              </button>
            </div>
          </div>
        </div>
      )}
    </AdminLayout>
  );
};
