import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { condicaoJogoApi, generoApi, jogoApi, plataformaApi } from '../api/servicos';
import { obterErrosDosCampos, obterMensagemErro } from '../api/erros';
import { Carregando } from '../components/MensagensEstado';
const inicial = { nome: '', descricao: '', anoLancamento: 1990, preco: '', estoque: 0, idPlataforma: '', idCondicaoJogo: '', idsGeneros: [], ativo: true };
export default function FormularioJogo() {
  const { id } = useParams(); const editar = !!id; const navegar = useNavigate(); const [modelo, setModelo] = useState(inicial); const [listas, setListas] = useState(null); const [erro, setErro] = useState(''); const [errosCampo, setErrosCampo] = useState({}); const [salvando, setSalvando] = useState(false);
  useEffect(() => { Promise.all([plataformaApi.listar(), generoApi.listar(), condicaoJogoApi.listar(), editar ? jogoApi.buscarPorId(id) : Promise.resolve(null)]).then(([plataformas, generos, condicoes, jogo]) => { setListas({ plataformas, generos, condicoes }); if (jogo) setModelo({ nome: jogo.nome, descricao: jogo.descricao || '', anoLancamento: jogo.anoLancamento, preco: jogo.preco, estoque: jogo.estoque, idPlataforma: jogo.idPlataforma, idCondicaoJogo: jogo.idCondicaoJogo, idsGeneros: jogo.generos.map((g) => g.id), ativo: jogo.ativo }); }).catch((e) => setErro(obterMensagemErro(e))); }, [id, editar]);
  function alterar(chave, valor) { setModelo((atual) => ({ ...atual, [chave]: valor })); }
  async function salvar(e) { e.preventDefault(); setSalvando(true); setErro(''); setErrosCampo({}); const dados = { ...modelo, anoLancamento: Number(modelo.anoLancamento), preco: Number(modelo.preco), estoque: Number(modelo.estoque), idPlataforma: Number(modelo.idPlataforma), idCondicaoJogo: Number(modelo.idCondicaoJogo) }; if (editar) dados.ativo = modelo.ativo; else delete dados.ativo; try { editar ? await jogoApi.atualizar(id, dados) : await jogoApi.inserir(dados); navegar('/admin/jogos'); } catch (ex) { setErro(obterMensagemErro(ex)); setErrosCampo(obterErrosDosCampos(ex)); } finally { setSalvando(false); } }
  if (!listas && !erro) return <Carregando />;
  const invalidClass = (chave) => errosCampo[chave] ? ' is-invalid' : '';
  const mensagens = (chave) => errosCampo[chave]?.map((m, i) => <div className="invalid-feedback d-block" id={i === 0 ? `erro-${chave}` : undefined} key={m}>{m}</div>);
  const ligacao = (chave) => ({ 'aria-invalid': errosCampo[chave] ? 'true' : undefined, 'aria-describedby': errosCampo[chave] ? `erro-${chave}` : undefined });
  return <>
    <Link to="/admin/jogos" className="btn btn-sm btn-outline-dark">← Voltar aos jogos</Link>
    <h1 className="h2 mt-3 mb-3">{editar ? 'Editar jogo' : 'Novo jogo'}</h1>
    {erro && <div className="alert alert-danger" role="alert">{erro}</div>}
    <form className="card rg-painel rg-formulario" onSubmit={salvar} noValidate>
      <div className="card-body p-3 p-md-4">
        <div className="row g-3">
          <div className="col-12 col-md-8"><label className="form-label" htmlFor="jogo-nome">Nome</label><input id="jogo-nome" className={`form-control${invalidClass('nome')}`} value={modelo.nome} onChange={(e) => alterar('nome', e.target.value)} maxLength="150" required {...ligacao('nome')} />{mensagens('nome')}</div>
          <div className="col-12 col-md-4"><label className="form-label" htmlFor="jogo-ano">Ano de lançamento</label><input id="jogo-ano" className={`form-control${invalidClass('anoLancamento')}`} type="number" inputMode="numeric" min="1970" max="2100" value={modelo.anoLancamento} onChange={(e) => alterar('anoLancamento', e.target.value)} required {...ligacao('anoLancamento')} />{mensagens('anoLancamento')}</div>
          <div className="col-12"><label className="form-label" htmlFor="jogo-descricao">Descrição</label><textarea id="jogo-descricao" className="form-control" rows="3" maxLength="1000" value={modelo.descricao} onChange={(e) => alterar('descricao', e.target.value)} /></div>
          <div className="col-12 col-md-6"><label className="form-label" htmlFor="jogo-plataforma">Plataforma</label><select id="jogo-plataforma" className={`form-select${invalidClass('idPlataforma')}`} value={modelo.idPlataforma} onChange={(e) => alterar('idPlataforma', e.target.value)} required {...ligacao('idPlataforma')}><option value="">Selecione</option>{listas?.plataformas.map((x) => <option key={x.id} value={x.id}>{x.nome}</option>)}</select>{mensagens('idPlataforma')}</div>
          <div className="col-12 col-md-6"><label className="form-label" htmlFor="jogo-condicao">Condição</label><select id="jogo-condicao" className={`form-select${invalidClass('idCondicaoJogo')}`} value={modelo.idCondicaoJogo} onChange={(e) => alterar('idCondicaoJogo', e.target.value)} required {...ligacao('idCondicaoJogo')}><option value="">Selecione</option>{listas?.condicoes.map((x) => <option key={x.id} value={x.id}>{x.descricao}</option>)}</select>{mensagens('idCondicaoJogo')}</div>
          <div className="col-12 col-sm-6"><label className="form-label" htmlFor="jogo-preco">Preço (R$)</label><input id="jogo-preco" className={`form-control${invalidClass('preco')}`} type="number" inputMode="decimal" min="0.01" step="0.01" value={modelo.preco} onChange={(e) => alterar('preco', e.target.value)} required {...ligacao('preco')} />{mensagens('preco')}</div>
          <div className="col-12 col-sm-6"><label className="form-label" htmlFor="jogo-estoque">Estoque</label><input id="jogo-estoque" className={`form-control${invalidClass('estoque')}`} type="number" inputMode="numeric" min="0" value={modelo.estoque} onChange={(e) => alterar('estoque', e.target.value)} required {...ligacao('estoque')} />{mensagens('estoque')}</div>
          <div className="col-12"><fieldset><legend className="form-label">Gêneros</legend><div className="rg-generos">{listas?.generos.map((g) => <label className="rg-genero" key={g.id}><input className="form-check-input" type="checkbox" checked={modelo.idsGeneros.includes(g.id)} onChange={(e) => alterar('idsGeneros', e.target.checked ? [...modelo.idsGeneros, g.id] : modelo.idsGeneros.filter((x) => x !== g.id))} /><span>{g.nome}</span></label>)}</div>{mensagens('idsGeneros')}</fieldset></div>
          {editar && <div className="col-12"><div className="form-check form-switch"><input id="ativo" className="form-check-input" type="checkbox" role="switch" checked={modelo.ativo} onChange={(e) => alterar('ativo', e.target.checked)} /><label className="form-check-label fw-bold" htmlFor="ativo">Produto ativo</label></div><div className="form-text">Produtos inativos não aparecem no catálogo público. Ative novamente quando quiser.</div></div>}
        </div>
      </div>
      <div className="card-footer p-3 p-md-4 d-grid d-sm-flex justify-content-sm-end gap-2">
        <Link to="/admin/jogos" className="btn btn-outline-dark">Cancelar</Link>
        <button className="btn btn-retro" disabled={salvando}>{salvando ? 'Salvando...' : 'Salvar jogo'}</button>
      </div>
    </form>
  </>;
}
