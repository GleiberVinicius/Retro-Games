import { useEffect, useRef, useState } from 'react';
import { Link, NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom';
import { useAutenticacao } from '../hooks/useAutenticacao';
import logo from '../assets/logo-retro-games.png';

export default function Layout() {
  const { usuario, ehAdministrador, sair } = useAutenticacao();
  const navegar = useNavigate();
  const local = useLocation();
  const [menuAberto, setMenuAberto] = useState(false);
  const [adminAberto, setAdminAberto] = useState(false);
  const adminRef = useRef(null);

  async function encerrarSessao() { try { await sair(); } finally { navegar('/'); } }

  // Ao trocar de página, recolhe o menu (importante no celular).
  useEffect(() => { setMenuAberto(false); setAdminAberto(false); }, [local.pathname]);

  // Fecha o submenu Administração ao clicar fora dele ou apertar Esc.
  useEffect(() => {
    if (!adminAberto) return undefined;
    const fecharFora = (evento) => { if (adminRef.current && !adminRef.current.contains(evento.target)) setAdminAberto(false); };
    const fecharComEsc = (evento) => { if (evento.key === 'Escape') setAdminAberto(false); };
    document.addEventListener('mousedown', fecharFora);
    document.addEventListener('keydown', fecharComEsc);
    return () => { document.removeEventListener('mousedown', fecharFora); document.removeEventListener('keydown', fecharComEsc); };
  }, [adminAberto]);

  const noAdmin = local.pathname.startsWith('/admin');

  return (
    <div className="d-flex flex-column min-vh-100">
      <a className="rg-pular visually-hidden-focusable" href="#conteudo">Ir para o conteúdo</a>

      <header className="rg-cabecalho">
        <nav className="navbar navbar-expand-lg" aria-label="Navegação principal">
          <div className="container">
            <Link className="rg-marca" to="/">
              <img src={logo} alt="" width="48" height="48" />
              <span>RetroGames</span>
            </Link>
            <button
              className="navbar-toggler"
              type="button"
              aria-controls="menu"
              aria-expanded={menuAberto}
              aria-label="Alternar menu de navegação"
              onClick={() => setMenuAberto((atual) => !atual)}
            >
              <span className="navbar-toggler-icon" />
            </button>
            <div className={`collapse navbar-collapse${menuAberto ? ' show' : ''}`} id="menu">
              <div className="navbar-nav me-auto">
                <NavLink className="nav-link" to="/" end>Catálogo</NavLink>
                {ehAdministrador && (
                  <div className="nav-item dropdown" ref={adminRef}>
                    <button
                      type="button"
                      className={`nav-link dropdown-toggle${noAdmin ? ' active' : ''}${adminAberto ? ' show' : ''}`}
                      aria-expanded={adminAberto}
                      aria-haspopup="true"
                      onClick={() => setAdminAberto((atual) => !atual)}
                    >
                      Administração
                    </button>
                    <div className={`dropdown-menu${adminAberto ? ' show' : ''}`}>
                      <NavLink className="dropdown-item" to="/admin/jogos">Jogos</NavLink>
                      <NavLink className="dropdown-item" to="/admin/plataformas">Plataformas</NavLink>
                    </div>
                  </div>
                )}
              </div>
              <div className="navbar-nav flex-lg-row align-items-lg-center gap-2 rg-acoes-usuario">
                {usuario ? (
                  <>
                    <span className="rg-usuario me-lg-2">Olá, {usuario.nome}</span>
                    <button className="btn btn-sm btn-outline-dark" type="button" onClick={encerrarSessao}>Sair</button>
                  </>
                ) : (
                  <Link className="btn btn-sm btn-retro" to="/login">Entrar</Link>
                )}
              </div>
            </div>
          </div>
        </nav>
        <div className="rg-faixa" aria-hidden="true" />
      </header>

      <main id="conteudo" tabIndex={-1} className="container py-4 py-lg-5 flex-grow-1">
        <Outlet />
      </main>

      <footer className="rg-rodape">
        <div className="rg-faixa" aria-hidden="true" />
        <div className="container rg-rodape-corpo">
          <img src={logo} alt="" width="36" height="36" />
          <span>RetroGames · Colecione histórias.</span>
        </div>
      </footer>
    </div>
  );
}
