export default function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) return null;

  return <nav aria-label="Paginação"><ul className="pagination justify-content-center">
    <li className={`page-item ${pagina <= 1 ? 'disabled' : ''}`}>
      <button className="page-link" type="button" disabled={pagina <= 1} onClick={() => aoMudar(pagina - 1)}>Anterior</button>
    </li>
    {Array.from({ length: totalPaginas }, (_, indice) => indice + 1).map((numero) => <li className={`page-item ${pagina === numero ? 'active' : ''}`} key={numero}>
      <button className="page-link" type="button" aria-current={pagina === numero ? 'page' : undefined} onClick={() => aoMudar(numero)}>{numero}</button>
    </li>)}
    <li className={`page-item ${pagina >= totalPaginas ? 'disabled' : ''}`}>
      <button className="page-link" type="button" disabled={pagina >= totalPaginas} onClick={() => aoMudar(pagina + 1)}>Próxima</button>
    </li>
  </ul></nav>;
}
