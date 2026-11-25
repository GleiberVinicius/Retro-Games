import { Link } from 'react-router-dom';
export default function NaoEncontrada() {
  return <div className="rg-painel rg-painel-menta text-center p-4 p-md-5 mx-auto" style={{ maxWidth: '36rem' }}>
    <div className="rg-404" aria-hidden="true">404</div>
    <h1 className="h3 mt-3">Essa página saiu do cartucho.</h1>
    <p className="text-secondary">Não encontramos o endereço solicitado.</p>
    <Link className="btn btn-retro" to="/">Voltar ao catálogo</Link>
  </div>;
}
