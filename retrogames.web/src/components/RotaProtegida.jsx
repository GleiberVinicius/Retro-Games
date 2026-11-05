import { Navigate, Outlet, useLocation } from 'react-router';
import { useAutenticacao } from '../hooks/useAutenticacao';
export default function RotaProtegida({ somenteAdministrador = false }) { const { carregando, estaAutenticado, ehAdministrador } = useAutenticacao(); const local = useLocation(); if (carregando) return <p className="text-center py-5">Verificando sessão...</p>; if (!estaAutenticado) return <Navigate to={`/login?voltarPara=${encodeURIComponent(local.pathname)}`} replace />; if (somenteAdministrador && !ehAdministrador) return <Navigate to="/" replace />; return <Outlet />; }
