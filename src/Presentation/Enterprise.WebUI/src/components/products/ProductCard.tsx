import React from 'react';
import type { ProductDto } from '../../types/product.types';
import { ShoppingBag, Box, Check } from 'lucide-react';
import { useCart } from '../../context/CartContext';

interface ProductCardProps {
  product: ProductDto;
}

export const ProductCard: React.FC<ProductCardProps> = ({ product }) => {
  const { addToCart, cartItems } = useCart();
  const inCartItem = cartItems.find(item => item.product.id === product.id);

  return (
    <div className="glass-card" style={{ display: 'flex', flexDirection: 'column', height: '100%', overflow: 'hidden' }}>
      
      {/* Card Image Header */}
      <div style={{ position: 'relative', height: '200px', overflow: 'hidden', background: '#1e293b' }}>
        <img
          src={product.imageUrl || 'https://images.unsplash.com/photo-1581092160607-ee22621dd758?auto=format&fit=crop&w=600&q=80'}
          alt={product.name}
          style={{ width: '100%', height: '100%', objectFit: 'cover', transition: 'transform 0.5s ease' }}
        />
        <div style={{ position: 'absolute', top: '12px', left: '12px', display: 'flex', gap: '8px' }}>
          <span className="badge badge-cyan">{product.categoryName || 'Genel'}</span>
        </div>
        <div style={{ position: 'absolute', top: '12px', right: '12px' }}>
          <span className="badge badge-indigo" style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
            <Box size={12} />
            {product.sku}
          </span>
        </div>
      </div>

      {/* Card Content */}
      <div style={{ padding: '20px', flex: 1, display: 'flex', flexDirection: 'column' }}>
        <h3 style={{ fontSize: '1.1rem', fontWeight: 700, color: '#f3f4f6', marginBottom: '8px', lineHeight: 1.3 }}>
          {product.name}
        </h3>
        
        <p style={{ fontSize: '0.85rem', color: '#9ca3af', marginBottom: '20px', flex: 1, lineHeight: 1.5, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>
          {product.description}
        </p>

        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '16px', paddingTop: '12px', borderTop: '1px solid var(--border-glass)' }}>
          <div>
            <div style={{ fontSize: '0.75rem', color: '#6b7280' }}>Stok Durumu</div>
            <div style={{ fontSize: '0.85rem', fontWeight: 600, color: product.stockQuantity > 0 ? '#10b981' : '#f43f5e' }}>
              {product.stockQuantity > 0 ? `${product.stockQuantity} Adet Mevcut` : 'Stokta Yok'}
            </div>
          </div>

          <div style={{ textAlign: 'right' }}>
            <div style={{ fontSize: '0.75rem', color: '#6b7280' }}>Fiyat</div>
            <div style={{ fontSize: '1.2rem', fontWeight: 800, color: '#06b6d4' }}>
              {product.price.amount.toLocaleString('tr-TR')} <span style={{ fontSize: '0.85rem', fontWeight: 600 }}>{product.price.currency}</span>
            </div>
          </div>
        </div>

        {/* Action Button */}
        <button
          onClick={() => addToCart(product)}
          disabled={product.stockQuantity <= 0}
          className={inCartItem ? 'btn-secondary' : 'btn-primary'}
          style={{ width: '100%', justifyContent: 'center', padding: '12px' }}
        >
          {inCartItem ? (
            <>
              <Check size={18} color="#10b981" />
              <span>Sepette ({inCartItem.quantity})</span>
            </>
          ) : (
            <>
              <ShoppingBag size={18} />
              <span>Sepete Ekle</span>
            </>
          )}
        </button>

      </div>

    </div>
  );
};
