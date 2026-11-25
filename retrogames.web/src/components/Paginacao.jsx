// Mostra até 7 botões de página (com reticências) para não estourar a largura em telas pequenas.
function montarPaginas(pagina, totalPaginas) {
  if (totalPaginas <= 7) return Array.from({ length: totalPaginas }, (_, indice) => indice + 1);
  const itens = [1];
  const inicio = Math.max(2, pagina - 1);
  const fim = Math.min(totalPaginas - 1, pagina + 1);
  if (inicio > 2) itens.push('...inicio');
  for (let numero = inicio; numero <= fim; numero += 1) itens.push(numero);
  if (fim < totalPaginas - 1) itens.push('...fim');
  itens.push(totalPaginas);
  return itens;
}

export default function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) return null;

  return <nav aria-label="Paginação"><ul className="pagination justify-content-center">
    <li className={`page-item ${pagina <= 1 ? 'disabled' : ''}`}>
      <button className="page-link" type="button" disabled={pagina <= 1} onClick={() => aoMudar(pagina - 1)}>Anterior</button>
    </li>
    <li className="d-sm-none rg-pagina-status" aria-live="polite">Página {pagina} de {totalPaginas}</li>
    {montarPaginas(pagina, totalPaginas).map((numero) => typeof numero === 'string'
      ? <li className="page-item disabled d-none d-sm-block" key={numero} aria-hidden="true"><span className="page-link">…</span></li>
      : <li className={`page-item d-none d-sm-block ${pagina === numero ? 'active' : ''}`} key={numero}>
        <button className="page-link" type="button" aria-label={`Página ${numero}`} aria-current={pagina === numero ? 'page' : undefined} onClick={() => aoMudar(numero)}>{numero}</button>
      </li>)}
    <li className={`page-item ${pagina >= totalPaginas ? 'disabled' : ''}`}>
      <button className="page-link" type="button" disabled={pagina >= totalPaginas} onClick={() => aoMudar(pagina + 1)}>Próxima</button>
    </li>
  </ul></nav>;
}
