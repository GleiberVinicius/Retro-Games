import { useCallback, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { jogoApi } from '../api/servicos';
import { obterMensagemErro } from '../api/erros';
import { Carregando, EstadoVazio, MensagemErro } from '../components/MensagensEstado';
import Paginacao from '../components/Paginacao';
export default function Jogos() {
  const [resultado, setResultado] = useState(null); const [erro, setErro] = useState(''); const [pagina, setPagina] = useState(1); const [busca, setBusca] = useState('');
  const carregar = useCallback(async () => { setErro(''); try { setResultado(await jogoApi.listar({ busca: busca || undefined, pagina, tamanhoPagina: 15, incluirInativos: true })); } catch (e) { setErro(obterMensagemErro(e)); } }, [busca, pagina]);
  useEffect(() => { carregar(); }, [carregar]);
  async function desativar(id) { if (!window.confirm('Deseja desativar este jogo?')) return; try { await jogoApi.excluir(id); await carregar(); } catch (e) { setErro(obterMensagemErro(e)); } }

  const situacao = (j) => <span className={`rg-situacao ${j.ativo ? 'rg-situacao-ativo' : 'rg-situacao-inativo'}`}>{j.ativo ? 'Ativo' : 'Inativo'}</span>;
  const acoes = (j) => <div className="rg-acoes">
    <Link className="btn btn-sm btn-outline-dark" to={`/admin/jogos/${j.id}/editar`} aria-label={`Editar ${j.nome}`}>Editar</Link>
    {j.ativo && <button className="btn btn-sm btn-outline-danger" type="button" onClick={() => desativar(j.id)} aria-label={`Desativar ${j.nome}`}>Desativar</button>}
  </div>;

  return <>
    <div className="d-flex flex-column flex-sm-row justify-content-between align-items-sm-center gap-3 mb-3">
      <div><h1 className="h2 mb-1">Jogos</h1><p className="text-secondary mb-0">Gerencie os itens do catálogo.</p></div>
      <Link to="/admin/jogos/novo" className="btn btn-retro">Novo jogo</Link>
    </div>
    <div className="rg-painel rg-filtros mb-3">
      <label className="form-label" htmlFor="busca-admin">Buscar jogo</label>
      <input id="busca-admin" className="form-control" type="search" placeholder="Buscar jogo..." value={busca} onChange={(e) => { setBusca(e.target.value); setPagina(1); }} />
    </div>
    {erro && <MensagemErro mensagem={erro} aoTentarNovamente={carregar} />}
    {!resultado && !erro ? <Carregando /> : resultado?.itens?.length ? <>
      {/* Telas médias e grandes: tabela */}
      <div className="rg-painel rg-tabela table-responsive d-none d-md-block">
        <table className="table align-middle">
          <thead><tr><th scope="col">Jogo</th><th scope="col">Plataforma</th><th scope="col" className="text-end">Estoque</th><th scope="col">Situação</th><th scope="col">Ações</th></tr></thead>
          <tbody>{resultado.itens.map((j) => <tr key={j.id}>
            <td><strong>{j.nome}</strong><div className="small text-secondary">{j.anoLancamento}</div></td>
            <td>{j.plataforma}</td>
            <td className="text-end">{j.estoque}</td>
            <td>{situacao(j)}</td>
            <td>{acoes(j)}</td>
          </tr>)}</tbody>
        </table>
      </div>
      {/* Celular: um cartão por jogo, com todas as informações e ações */}
      <div className="rg-painel rg-lista-admin overflow-hidden d-md-none">
        <ul className="list-unstyled mb-0">{resultado.itens.map((j) => <li className="rg-item-admin" key={j.id}>
          <div className="d-flex justify-content-between align-items-start gap-2 mb-2">
            <div><strong>{j.nome}</strong><div className="small text-secondary">{j.anoLancamento}</div></div>
            {situacao(j)}
          </div>
          <dl className="row small mb-3 g-1">
            <dt className="col-5 text-secondary fw-semibold">Plataforma</dt><dd className="col-7 mb-0">{j.plataforma}</dd>
            <dt className="col-5 text-secondary fw-semibold">Estoque</dt><dd className="col-7 mb-0">{j.estoque}</dd>
          </dl>
          {acoes(j)}
        </li>)}</ul>
      </div>
      <div className="mt-3"><Paginacao pagina={pagina} totalPaginas={resultado.totalPaginas} aoMudar={setPagina} /></div>
    </> : !erro && <EstadoVazio titulo="Nenhum jogo cadastrado">Use o botão “Novo jogo” para começar.</EstadoVazio>}
  </>;
}
