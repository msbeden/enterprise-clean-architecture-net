import React, { createContext, useContext, useState, useEffect } from 'react';
import type { AuthUser, LoginCommand } from '../types/auth.types';
import { apiClient } from '../services/api';

interface AuthContextType {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (cmd: LoginCommand) => Promise<{ success: boolean; message?: string }>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<AuthUser | null>(() => {
    const storedUser = localStorage.getItem('app_user');
    return storedUser ? JSON.parse(storedUser) : null;
  });

  const [token, setToken] = useState<string | null>(() => {
    return localStorage.getItem('app_access_token');
  });

  useEffect(() => {
    if (token) {
      localStorage.setItem('app_access_token', token);
    } else {
      localStorage.removeItem('app_access_token');
    }
  }, [token]);

  useEffect(() => {
    if (user) {
      localStorage.setItem('app_user', JSON.stringify(user));
    } else {
      localStorage.removeItem('app_user');
    }
  }, [user]);

  const login = async (cmd: LoginCommand) => {
    const res = await apiClient.login(cmd);
    if (res.isSuccess && res.value) {
      const authData = res.value;
      const loggedUser: AuthUser = {
        userId: authData.userId,
        email: authData.email,
        firstName: authData.firstName,
        lastName: authData.lastName,
        role: authData.role
      };
      setToken(authData.token);
      setUser(loggedUser);
      return { success: true };
    }
    return {
      success: false,
      message: res.error?.description || 'Giriş başarısız. Lütfen bilgilerinizi kontrol ediniz.'
    };
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem('app_access_token');
    localStorage.removeItem('app_user');
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token && !!user,
        login,
        logout
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
