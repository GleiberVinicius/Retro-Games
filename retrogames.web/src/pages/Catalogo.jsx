import { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { jogoApi, plataformaApi, generoApi } from '../api/servicos';
import { obterMensagemErro } from '../api/erros';
import CardJogo from '../components/CardJogo';
import Paginacao from '../components/Paginacao';
import { Carregando, EstadoVazio, MensagemErro } from '../components/MensagensEstado';
import logo from '../assets/logo-retro-games.png';
export default function Catalogo() {
  const [parametros, setParametros] = useSearchParams(); const [jogos, setJogos] = useState(null); const [plataformas, setPlataformas] = useState([]); const [generos, setGeneros] = useState([]); const [erro, setErro] = useState('');
  const busca = parametros.get('busca') || ''; const idPlataforma = parametros.get('idPlataforma') || ''; const idGenero = parametros.get('idGenero') || ''; const pagina = Number(parametros.get('pagina') || 1);
  useEffect(() => { plataformaApi.listar().then(setPlataformas).catch(() => {}); generoApi.listar().then(setGeneros).catch(() => {}); }, []);
  async function carregar() { setErro(''); try { setJogos(await jogoApi.listar({ busca: busca || undefined, idPlataforma: idPlataforma || undefined, idGenero: idGenero || undefined, pagina, tamanhoPagina: 12 })); } catch (e) { setErro(obterMensagemErro(e)); } }
  useEffect(() => { carregar(); }, [busca, idPlataforma, idGenero, pagina]);
  function alterar(chave, valor) { const novo = new URLSearchParams(parametros); valor ? novo.set(chave, valor) : novo.delete(chave); if (chave !== 'pagina') novo.delete('pagina'); setParametros(novo); }
  return <>
    <section className="rg-painel rg-hero mb-4" aria-labelledby="titulo-catalogo">
      <div className="rg-hero-corpo d-flex align-items-center justify-content-between gap-4">
        <div>
          <p className="rg-hero-etiqueta">Sua próxima viagem no tempo</p>
          <h1 id="titulo-catalogo">Clássicos que continuam jogáveis.</h1>
          <p className="mb-0 fs-5">Explore consoles, gêneros e jogos retrô selecionados.</p>
        </div>
        <img className="rg-hero-logo d-none d-md-block" src={logo} alt="Logo RetroGames: controle de videogame sobre listras coloridas" width="200" height="200" />
      </div>
      <div className="rg-faixa" aria-hidden="true" />
    </section>

    <section className="rg-painel rg-filtros mb-4" aria-label="Busca e filtros">
      <div className="row g-3">
        <div className="col-12 col-md-6">
          <label className="form-label" htmlFor="filtro-busca">Buscar jogo</label>
          <input id="filtro-busca" className="form-control" type="search" placeholder="Buscar por nome..." value={busca} onChange={(e) => alterar('busca', e.target.value)} />
        </div>
        <div className="col-12 col-sm-6 col-md-3">
          <label className="form-label" htmlFor="filtro-plataforma">Plataforma</label>
          <select id="filtro-plataforma" className="form-select" value={idPlataforma} onChange={(e) => alterar('idPlataforma', e.target.value)}><option value="">Todas as plataformas</option>{plataformas?.map((p) => <option key={p.id} value={p.id}>{p.nome}</option>)}</select>
        </div>
        <div className="col-12 col-sm-6 col-md-3">
          <label className="form-label" htmlFor="filtro-genero">Gênero</label>
          <select id="filtro-genero" className="form-select" value={idGenero} onChange={(e) => alterar('idGenero', e.target.value)}><option value="">Todos os gêneros</option>{generos?.map((g) => <option key={g.id} value={g.id}>{g.nome}</option>)}</select>
        </div>
      </div>
    </section>

    {erro && <MensagemErro mensagem={erro} aoTentarNovamente={carregar} />}
    {!jogos && !erro ? <Carregando texto="Carregando catálogo..." /> : jogos?.itens?.length ? <>
      <div className="row row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xxl-4 g-3 g-lg-4 mb-4">{jogos.itens.map((jogo) => <div className="col" key={jogo.id}><CardJogo jogo={jogo} /></div>)}</div>
      <Paginacao pagina={pagina} totalPaginas={jogos.totalPaginas} aoMudar={(n) => alterar('pagina', String(n))} />
    </> : !erro && <EstadoVazio titulo="Nenhum jogo encontrado">Tente alterar a busca ou os filtros.</EstadoVazio>}
  </>;
}
