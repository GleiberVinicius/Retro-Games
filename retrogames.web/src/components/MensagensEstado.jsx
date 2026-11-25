export function Carregando({ texto = 'Carregando...' }) {
  return <div className="rg-estado" role="status"><span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />{texto}</div>;
}
export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return <div className="alert alert-danger d-flex flex-column flex-sm-row justify-content-between align-items-sm-center gap-2" role="alert"><span>{mensagem}</span>{aoTentarNovamente && <button className="btn btn-sm btn-outline-danger" type="button" onClick={aoTentarNovamente}>Tentar novamente</button>}</div>;
}
export function EstadoVazio({ titulo, children }) {
  return <div className="rg-vazio"><h2 className="h5">{titulo}</h2><p className="text-secondary mb-0">{children}</p></div>;
}
