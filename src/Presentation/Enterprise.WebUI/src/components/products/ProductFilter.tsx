import React from 'react';
import type { CategoryDto } from '../../types/category.types';
import { Search, Grid, Filter } from 'lucide-react';

interface ProductFilterProps {
  categories: CategoryDto[];
  selectedCategoryId: string;
  onSelectCategory: (id: string) => void;
  searchTerm: string;
  onSearchChange: (term: string) => void;
}

export const ProductFilter: React.FC<ProductFilterProps> = ({
  categories,
  selectedCategoryId,
  onSelectCategory,
  searchTerm,
  onSearchChange
}) => {
  return (
    <div className="glass-panel" style={{ padding: '20px', marginBottom: '32px' }}>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
        
        {/* Search Bar */}
        <div style={{ position: 'relative' }}>
          <Search size={20} color="#9ca3af" style={{ position: 'absolute', left: '16px', top: '50%', transform: 'translateY(-50%)' }} />
          <input
            type="text"
            className="form-control"
            style={{ paddingLeft: '48px', height: '48px', fontSize: '1rem' }}
            placeholder="Ürün adı, parça numarası veya SKU ile arama yapın..."
            value={searchTerm}
            onChange={e => onSearchChange(e.target.value)}
          />
        </div>

        {/* Category Pills */}
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '12px', fontSize: '0.85rem', fontWeight: 600, color: '#9ca3af' }}>
            <Filter size={16} color="#06b6d4" />
            <span>Kategoriler:</span>
          </div>

          <div style={{ display: 'flex', flexWrap: 'wrap', gap: '10px' }}>
            <button
              onClick={() => onSelectCategory('')}
              className={selectedCategoryId === '' ? 'btn-primary' : 'btn-secondary'}
              style={{ padding: '8px 16px', borderRadius: '9999px', fontSize: '0.85rem' }}
            >
              <Grid size={14} />
              Tüm Ürünler
            </button>

            {categories.map(cat => (
              <button
                key={cat.id}
                onClick={() => onSelectCategory(cat.id)}
                className={selectedCategoryId === cat.id ? 'btn-primary' : 'btn-secondary'}
                style={{ padding: '8px 16px', borderRadius: '9999px', fontSize: '0.85rem' }}
              >
                {cat.name}
              </button>
            ))}
          </div>
        </div>

      </div>
    </div>
  );
};
