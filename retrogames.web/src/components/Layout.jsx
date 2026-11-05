import { Link, NavLink, Outlet, useNavigate } from 'react-router';
import { useAutenticacao } from '../hooks/useAutenticacao';
export default function Layout() {
  const { usuario, ehAdministrador, sair } = useAutenticacao(); const navegar = useNavigate();
  async function encerrarSessao() { try { await sair(); } finally { navegar('/'); } }
  return <><nav className="navbar navbar-expand-lg navbar-dark"><div className="container"><Link className="navbar-brand marca" to="/">🕹 RetroGames</Link><button className="navbar-toggler" data-bs-toggle="collapse" data-bs-target="#menu"><span className="navbar-toggler-icon" /></button><div className="collapse navbar-collapse" id="menu"><div className="navbar-nav me-auto"><NavLink className="nav-link" to="/">Catálogo</NavLink>{ehAdministrador && <><NavLink className="nav-link" to="/admin/jogos">Jogos</NavLink><NavLink className="nav-link" to="/admin/plataformas">Plataformas</NavLink></>}</div><div className="navbar-nav align-items-lg-center">{usuario ? <><span className="navbar-text me-lg-3">Olá, {usuario.nome}</span><button className="btn btn-sm btn-outline-light" onClick={encerrarSessao}>Sair</button></> : <Link className="btn btn-sm btn-light" to="/login">Entrar</Link>}</div></div></div></nav><main className="container py-4"><Outlet /></main><footer className="container rodape">RetroGames · Colecione histórias.</footer></>;
}
