import React, { useState, useEffect } from 'react';
import { AdminLayout } from '../../components/layout/AdminLayout';
import type { ProductDto } from '../../types/product.types';
import type { CategoryDto } from '../../types/category.types';
import { apiClient } from '../../services/api';
import { Plus, Package, X, CheckCircle, Edit, Trash2, AlertTriangle } from 'lucide-react';

export const AdminProductsPage: React.FC = () => {
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingProduct, setEditingProduct] = useState<ProductDto | null>(null);

  // Form State
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [sku, setSku] = useState('');
  const [priceAmount, setPriceAmount] = useState<number>(1000);
  const [stockQuantity, setStockQuantity] = useState<number>(10);
  const [categoryId, setCategoryId] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [formLoading, setFormLoading] = useState(false);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  // Delete modal state
  const [deletingProduct, setDeletingProduct] = useState<ProductDto | null>(null);
  const [deleteLoading, setDeleteLoading] = useState(false);

  const loadData = () => {
    setLoading(true);
    apiClient.getCategories().then(cRes => {
      if (cRes.isSuccess && cRes.value) {
        setCategories(cRes.value);
        if (cRes.value.length > 0 && !categoryId) setCategoryId(cRes.value[0].id);
      }
    });

    apiClient.getProducts({ pageSize: 100 }).then(pRes => {
      setLoading(false);
      if (pRes.isSuccess && pRes.value) {
        setProducts(pRes.value.items);
      }
    });
  };

  useEffect(() => {
    loadData();
  }, []);

  const openCreateModal = () => {
    setEditingProduct(null);
    setName('');
    setDescription('');
    setSku('');
    setPriceAmount(1000);
    setStockQuantity(10);
    setImageUrl('');
    if (categories.length > 0) setCategoryId(categories[0].id);
    setSuccessMsg(null);
    setShowModal(true);
  };

  const openEditModal = (prod: ProductDto) => {
    setEditingProduct(prod);
    setName(prod.name);
    setDescription(prod.description);
    setSku(prod.sku);
    setPriceAmount(prod.price.amount);
    setStockQuantity(prod.stockQuantity);
    setCategoryId(prod.categoryId);
    setImageUrl(prod.imageUrl || '');
    setSuccessMsg(null);
    setShowModal(true);
  };

  const handleSaveProduct = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormLoading(true);
    setSuccessMsg(null);

    let res;
    if (editingProduct) {
      res = await apiClient.updateProduct({
        id: editingProduct.id,
        name,
        description,
        sku,
        priceAmount,
        currency: 'TRY',
        stockQuantity,
        categoryId,
        imageUrl
      });
    } else {
      res = await apiClient.createProduct({
        name,
        description,
        sku,
        priceAmount,
        currency: 'TRY',
        stockQuantity,
        categoryId,
        imageUrl
      });
    }

    setFormLoading(false);
    if (res.isSuccess) {
      setSuccessMsg(editingProduct ? 'Ürün başarıyla güncellendi!' : 'Ürün başarıyla eklendi!');
      loadData();
      setTimeout(() => {
        setShowModal(false);
        setSuccessMsg(null);
      }, 1000);
    }
  };

  const handleDelete = async () => {
    if (!deletingProduct) return;
    setDeleteLoading(true);
    const res = await apiClient.deleteProduct(deletingProduct.id);
    setDeleteLoading(false);
    if (res.isSuccess) {
      setDeletingProduct(null);
      loadData();
    }
  };

  return (
    <AdminLayout>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '28px' }}>
        <div>
          <h1 style={{ fontSize: '1.8rem', fontWeight: 800, color: '#f3f4f6', marginBottom: '4px' }}>Ürün Yönetimi</h1>
          <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>Sistemdeki tüm ürünlerin listesi, yeni ekleme, düzenleme ve silme işlemleri</p>
        </div>

        <button onClick={openCreateModal} className="btn-primary">
          <Plus size={18} />
          <span>Yeni Ürün Ekle</span>
        </button>
      </div>

      {/* Table Panel */}
      <div className="glass-panel" style={{ overflow: 'hidden' }}>
        {loading ? (
          <div style={{ padding: '40px', textAlign: 'center', color: '#9ca3af' }}>Ürünler yükleniyor...</div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Görsel & Ürün Adı</th>
                <th>SKU</th>
                <th>Kategori</th>
                <th>Fiyat</th>
                <th>Stok</th>
                <th>Durum</th>
                <th style={{ textAlign: 'right' }}>İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {products.map(prod => (
                <tr key={prod.id}>
                  <td>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                      <img src={prod.imageUrl || 'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=600&q=80'} alt={prod.name} style={{ width: '40px', height: '40px', objectFit: 'cover', borderRadius: '6px' }} />
                      <div>
                        <div style={{ fontWeight: 600, color: '#f3f4f6' }}>{prod.name}</div>
                        <div style={{ fontSize: '0.75rem', color: '#9ca3af' }}>{prod.id}</div>
                      </div>
                    </div>
                  </td>
                  <td>
                    <span className="badge badge-indigo">{prod.sku}</span>
                  </td>
                  <td>{prod.categoryName || 'Genel'}</td>
                  <td>
                    <strong style={{ color: '#06b6d4' }}>{prod.price.amount.toLocaleString('tr-TR')} {prod.price.currency}</strong>
                  </td>
                  <td>{prod.stockQuantity} Adet</td>
                  <td>
                    <span className={prod.stockQuantity > 0 ? 'badge badge-emerald' : 'badge badge-cyan'}>
                      {prod.stockQuantity > 0 ? 'Aktif' : 'Tükendi'}
                    </span>
                  </td>
                  <td style={{ textAlign: 'right' }}>
                    <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '8px' }}>
                      <button onClick={() => openEditModal(prod)} style={{ background: 'rgba(99,102,241,0.15)', border: '1px solid rgba(99,102,241,0.3)', color: '#818cf8', padding: '6px 10px', borderRadius: '6px', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '4px', fontSize: '0.8rem', fontWeight: 600 }}>
                        <Edit size={14} /> Düzenle
                      </button>
                      <button onClick={() => setDeletingProduct(prod)} style={{ background: 'rgba(239,68,68,0.15)', border: '1px solid rgba(239,68,68,0.3)', color: '#f87171', padding: '6px 10px', borderRadius: '6px', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '4px', fontSize: '0.8rem', fontWeight: 600 }}>
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

      {/* Create / Edit Product Modal */}
      {showModal && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 50, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '20px' }}>
          <div className="glass-panel" style={{ width: '100%', maxWidth: '560px', padding: '32px', background: '#0f172a' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
              <h3 style={{ fontSize: '1.3rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: '10px' }}>
                <Package color="#06b6d4" /> {editingProduct ? 'Ürünü Düzenle' : 'Yeni Ürün Oluştur'}
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

            <form onSubmit={handleSaveProduct} style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <div>
                <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Ürün Adı</label>
                <input required type="text" className="form-control" placeholder="Örn: ProBook Enterprise Laptop" value={name} onChange={e => setName(e.target.value)} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>SKU Kodu</label>
                <input required type="text" className="form-control" placeholder="Örn: HW-LAP-101" value={sku} onChange={e => setSku(e.target.value)} />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                <div>
                  <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Fiyat (TRY)</label>
                  <input required type="number" className="form-control" min={1} value={priceAmount} onChange={e => setPriceAmount(Number(e.target.value))} />
                </div>
                <div>
                  <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Stok Adedi</label>
                  <input required type="number" className="form-control" min={0} value={stockQuantity} onChange={e => setStockQuantity(Number(e.target.value))} />
                </div>
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Kategori Seçimi</label>
                <select className="form-control" value={categoryId} onChange={e => setCategoryId(e.target.value)}>
                  {categories.map(c => (
                    <option key={c.id} value={c.id} style={{ background: '#0f172a' }}>{c.name}</option>
                  ))}
                </select>
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Görsel URL (İsteğe Bağlı)</label>
                <input type="text" className="form-control" placeholder="https://..." value={imageUrl} onChange={e => setImageUrl(e.target.value)} />
              </div>

              <div>
                <label style={{ display: 'block', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af', marginBottom: '6px' }}>Ürün Açıklaması</label>
                <textarea required className="form-control" rows={3} placeholder="Ürün özelliklerini detaylandırın..." value={description} onChange={e => setDescription(e.target.value)} />
              </div>

              <button type="submit" disabled={formLoading} className="btn-primary" style={{ width: '100%', justifyContent: 'center', padding: '12px', marginTop: '8px' }}>
                {formLoading ? 'Kaydediliyor...' : editingProduct ? 'Değişiklikleri Kaydet' : 'Ürünü Kaydet'}
              </button>
            </form>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {deletingProduct && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 50, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '20px' }}>
          <div className="glass-panel" style={{ width: '100%', maxWidth: '420px', padding: '28px', background: '#0f172a', textAlign: 'center' }}>
            <div style={{ display: 'inline-flex', padding: '16px', background: 'rgba(239,68,68,0.15)', borderRadius: '50%', color: '#ef4444', marginBottom: '16px' }}>
              <AlertTriangle size={32} />
            </div>
            <h3 style={{ fontSize: '1.2rem', fontWeight: 700, marginBottom: '8px' }}>Ürünü Silmek İstediğinize Emin Misiniz?</h3>
            <p style={{ color: '#9ca3af', fontSize: '0.88rem', marginBottom: '24px' }}>
              <strong style={{ color: '#f3f4f6' }}>"{deletingProduct.name}"</strong> ürününü sistemden kalıcı olarak silmek üzeresiniz. Bu işlem geri alınamaz.
            </p>
            <div style={{ display: 'flex', gap: '12px', justifyContent: 'center' }}>
              <button onClick={() => setDeletingProduct(null)} style={{ padding: '10px 20px', borderRadius: '8px', border: '1px solid rgba(255,255,255,0.1)', background: 'transparent', color: '#9ca3af', cursor: 'pointer', fontWeight: 600 }}>
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
