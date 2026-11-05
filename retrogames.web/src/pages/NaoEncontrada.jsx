import { Link } from 'react-router';
export default function NaoEncontrada() { return <div className="text-center py-5"><div className="display-1 fw-bold">404</div><h1 className="h3">Essa página saiu do cartucho.</h1><p className="text-secondary">Não encontramos o endereço solicitado.</p><Link className="btn btn-dark" to="/">Voltar ao catálogo</Link></div>; }
