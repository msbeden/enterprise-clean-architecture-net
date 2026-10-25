import React, { useState, useEffect } from 'react';
import { MainLayout } from '../../components/layout/MainLayout';
import { ProductCard } from '../../components/products/ProductCard';
import { ProductFilter } from '../../components/products/ProductFilter';
import type { CategoryDto } from '../../types/category.types';
import type { ProductDto } from '../../types/product.types';
import { apiClient } from '../../services/api';
import { ChevronLeft, ChevronRight, PackageSearch, Sparkles } from 'lucide-react';

export const HomePage: React.FC = () => {
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<string>('');
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(1);
  const [totalCount, setTotalCount] = useState<number>(0);
  const [loading, setLoading] = useState<boolean>(true);

  // Load Categories on Mount
  useEffect(() => {
    apiClient.getCategories().then(res => {
      if (res.isSuccess && res.value) {
        setCategories(res.value);
      }
    });
  }, []);

  // Load Products whenever category, search, or page changes
  useEffect(() => {
    setLoading(true);
    apiClient.getProducts({
      pageNumber,
      pageSize: 6,
      searchTerm,
      categoryId: selectedCategory
    }).then(res => {
      setLoading(false);
      if (res.isSuccess && res.value) {
        setProducts(res.value.items);
        setTotalPages(res.value.totalPages);
        setTotalCount(res.value.totalCount);
      }
    });
  }, [selectedCategory, searchTerm, pageNumber]);

  const handleCategorySelect = (id: string) => {
    setSelectedCategory(id);
    setPageNumber(1);
  };

  const handleSearchChange = (term: string) => {
    setSearchTerm(term);
    setPageNumber(1);
  };

  return (
    <MainLayout>
      {/* Hero Section */}
      <div className="glass-panel" style={{ padding: '40px 32px', marginBottom: '32px', position: 'relative', overflow: 'hidden' }}>
        <div style={{ position: 'relative', zIndex: 2, maxWidth: '720px' }}>
          <div style={{ display: 'inline-flex', alignItems: 'center', gap: '8px', padding: '6px 14px', background: 'rgba(6,182,212,0.1)', border: '1px solid rgba(6,182,212,0.3)', borderRadius: '9999px', color: '#67e8f9', fontSize: '0.85rem', fontWeight: 600, marginBottom: '16px' }}>
            <Sparkles size={16} />
            Kurumsal Ürün Kataloğu
          </div>
          <h1 style={{ fontSize: '2.5rem', fontWeight: 800, lineHeight: 1.2, marginBottom: '16px', background: 'linear-gradient(135deg, #ffffff, #94a3b8)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}>
            En Son Teknolojik Donanım & Ürün Çözümleri
          </h1>
          <p style={{ color: '#9ca3af', fontSize: '1.05rem', lineHeight: 1.6 }}>
            Kategorilere göre filtrelenmiş geniş ürün yelpazemizi inceleyin, sepetinize ekleyin ve siparişlerinizi anında yönetin.
          </p>
        </div>
      </div>

      {/* Filter Component */}
      <ProductFilter
        categories={categories}
        selectedCategoryId={selectedCategory}
        onSelectCategory={handleCategorySelect}
        searchTerm={searchTerm}
        onSearchChange={handleSearchChange}
      />

      {/* Product Grid Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <h2 style={{ fontSize: '1.25rem', fontWeight: 700, color: '#f3f4f6' }}>
          Ürün Listesi <span style={{ fontSize: '0.9rem', color: '#6b7280', fontWeight: 500 }}>({totalCount} Ürün Bulundu)</span>
        </h2>
      </div>

      {/* Loading & Grid */}
      {loading ? (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))', gap: '24px' }}>
          {[1, 2, 3, 4, 5, 6].map(i => (
            <div key={i} className="glass-card" style={{ height: '380px', padding: '20px', display: 'flex', flexDirection: 'column', gap: '16px', opacity: 0.5 }}>
              <div style={{ height: '180px', background: 'rgba(255,255,255,0.05)', borderRadius: '8px' }} />
              <div style={{ height: '20px', background: 'rgba(255,255,255,0.05)', borderRadius: '4px', width: '70%' }} />
              <div style={{ height: '14px', background: 'rgba(255,255,255,0.05)', borderRadius: '4px', width: '100%' }} />
            </div>
          ))}
        </div>
      ) : products.length === 0 ? (
        <div className="glass-panel" style={{ textAlign: 'center', padding: '60px 20px', color: '#9ca3af' }}>
          <PackageSearch size={56} color="#64748b" style={{ marginBottom: '16px' }} />
          <h3 style={{ fontSize: '1.2rem', fontWeight: 700, color: '#f3f4f6', marginBottom: '8px' }}>Ürün Bulunamadı</h3>
          <p style={{ fontSize: '0.95rem' }}>Aradığınız kriterlere uygun ürün bulunmamaktadır. Lütfen arama terimini veya kategoriyi değiştirin.</p>
        </div>
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))', gap: '24px' }}>
          {products.map(product => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      )}

      {/* Pagination Bar */}
      {totalPages > 1 && (
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: '16px', marginTop: '40px' }}>
          <button
            onClick={() => setPageNumber(prev => Math.max(prev - 1, 1))}
            disabled={pageNumber <= 1}
            className="btn-secondary"
            style={{ padding: '8px 16px', opacity: pageNumber <= 1 ? 0.5 : 1 }}
          >
            <ChevronLeft size={18} />
            <span>Önceki</span>
          </button>

          <span style={{ fontSize: '0.9rem', color: '#9ca3af', fontWeight: 600 }}>
            Sayfa <strong style={{ color: '#06b6d4' }}>{pageNumber}</strong> / {totalPages}
          </span>

          <button
            onClick={() => setPageNumber(prev => Math.min(prev + 1, totalPages))}
            disabled={pageNumber >= totalPages}
            className="btn-secondary"
            style={{ padding: '8px 16px', opacity: pageNumber >= totalPages ? 0.5 : 1 }}
          >
            <span>Sonraki</span>
            <ChevronRight size={18} />
          </button>
        </div>
      )}
    </MainLayout>
  );
};
