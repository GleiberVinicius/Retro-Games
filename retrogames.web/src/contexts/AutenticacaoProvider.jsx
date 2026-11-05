import { useCallback, useEffect, useMemo, useState } from 'react';
import { autenticacaoApi } from '../api/servicos';
import { autenticacaoContext } from './autenticacaoContext';

export function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null);
  const [carregando, setCarregando] = useState(true);
  const atualizarUsuario = useCallback(async () => {
    try { setUsuario(await autenticacaoApi.eu()); } catch { setUsuario(null); } finally { setCarregando(false); }
  }, []);
  useEffect(() => { autenticacaoApi.csrf().catch(() => {}); atualizarUsuario(); const limpar = () => setUsuario(null); window.addEventListener('sessao:expirada', limpar); return () => window.removeEventListener('sessao:expirada', limpar); }, [atualizarUsuario]);
  const entrar = async (dados) => { const resultado = await autenticacaoApi.entrar(dados); setUsuario(resultado); return resultado; };
  const sair = async () => { await autenticacaoApi.sair(); setUsuario(null); };
  const valor = useMemo(() => ({ usuario, carregando, estaAutenticado: !!usuario, ehAdministrador: usuario?.perfil === 'Administrador', entrar, sair }), [usuario, carregando]);
  return <autenticacaoContext.Provider value={valor}>{children}</autenticacaoContext.Provider>;
}
