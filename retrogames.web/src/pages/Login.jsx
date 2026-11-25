import { useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { useAutenticacao } from '../hooks/useAutenticacao';
import { obterMensagemErro } from '../api/erros';
import logo from '../assets/logo-retro-games.png';
export default function Login() {
  const { entrar } = useAutenticacao(); const navegar = useNavigate(); const [params] = useSearchParams(); const [email, setEmail] = useState(''); const [senha, setSenha] = useState(''); const [erro, setErro] = useState(''); const [enviando, setEnviando] = useState(false);
  async function enviar(e) { e.preventDefault(); setEnviando(true); setErro(''); try { await entrar({ email, senha }); navegar(params.get('voltarPara') || '/', { replace: true }); } catch (ex) { setErro(obterMensagemErro(ex)); } finally { setEnviando(false); } }
  return <div className="row justify-content-center">
    <div className="col-12 col-sm-10 col-md-8 col-lg-5 col-xl-4">
      <div className="rg-painel rg-painel-rosa">
        <div className="p-4 p-sm-4">
          <img className="rg-login-logo" src={logo} alt="" width="110" height="110" />
          <h1 className="h3 mb-1 text-center">Entrar</h1>
          <p className="text-secondary text-center">Acesse sua conta de administrador.</p>
          {erro && <div className="alert alert-danger" role="alert">{erro}</div>}
          <form onSubmit={enviar}>
            <label className="form-label fw-bold" htmlFor="email">E-mail</label>
            <input id="email" className="form-control mb-3" type="email" autoComplete="username" required value={email} onChange={(e) => setEmail(e.target.value)} />
            <label className="form-label fw-bold" htmlFor="senha">Senha</label>
            <input id="senha" className="form-control mb-4" type="password" autoComplete="current-password" required value={senha} onChange={(e) => setSenha(e.target.value)} />
            <button className="btn btn-retro w-100" disabled={enviando}>{enviando ? 'Entrando...' : 'Entrar'}</button>
          </form>
        </div>
      </div>
      <p className="small text-secondary mt-3">A conta inicial deve ser provisionada conforme a configuração segura do ambiente.</p>
      <Link to="/" className="small fw-bold">Voltar ao catálogo</Link>
    </div>
  </div>;
}
