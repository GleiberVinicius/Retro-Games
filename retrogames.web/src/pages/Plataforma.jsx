import { useCallback, useEffect, useRef, useState } from 'react';
import { plataformaApi } from '../api/servicos';
import { obterErrosDosCampos, obterMensagemErro } from '../api/erros';
import { Carregando, EstadoVazio, MensagemErro } from '../components/MensagensEstado';

const vazio = { nome: '', fabricante: '', anoLancamento: 1990 };

export default function Plataforma() {
  const [lista, setLista] = useState(null);
  const [modelo, setModelo] = useState(vazio);
  const [idEdicao, setIdEdicao] = useState(null);
  const [erro, setErro] = useState('');
  const [errosCampo, setErrosCampo] = useState({});
  const formularioRef = useRef(null);

  const carregar = useCallback(async () => {
    try {
      setLista(await plataformaApi.listar());
      setErro('');
    } catch (ex) {
      setErro(obterMensagemErro(ex));
    }
  }, []);

  useEffect(() => {
    carregar();
  }, [carregar]);

  function alterar(campo, valor) {
    setModelo((atual) => ({ ...atual, [campo]: valor }));
  }

  async function salvar(evento) {
    evento.preventDefault();
    setErro('');
    setErrosCampo({});

    try {
      if (idEdicao) {
        await plataformaApi.atualizar(idEdicao, modelo);
      } else {
        await plataformaApi.inserir(modelo);
      }

      setModelo(vazio);
      setIdEdicao(null);
      await carregar();
    } catch (ex) {
      setErro(obterMensagemErro(ex));
      setErrosCampo(obterErrosDosCampos(ex));
    }
  }

  function editar(item) {
    setIdEdicao(item.id);
    setModelo({
      nome: item.nome,
      fabricante: item.fabricante,
      anoLancamento: item.anoLancamento,
    });
    setErro('');
    setErrosCampo({});
    // No celular o formulário fica acima da lista: leva a tela até ele e coloca o foco no nome.
    const reduzirMovimento = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    formularioRef.current?.scrollIntoView({ behavior: reduzirMovimento ? 'auto' : 'smooth', block: 'start' });
    formularioRef.current?.querySelector('#plataforma-nome')?.focus({ preventScroll: true });
  }

  async function excluir(id) {
    if (!window.confirm('Deseja excluir esta plataforma?')) return;

    try {
      await plataformaApi.excluir(id);
      await carregar();
    } catch (ex) {
      setErro(obterMensagemErro(ex));
    }
  }

  function classeInvalida(campo) {
    return errosCampo[campo] ? ' is-invalid' : '';
  }

  function mensagensDoCampo(campo) {
    return errosCampo[campo]?.map((mensagem, indice) => (
      <div className="invalid-feedback" id={indice === 0 ? `plataforma-erro-${campo}` : undefined} key={mensagem}>{mensagem}</div>
    ));
  }

  function ligacao(campo) {
    return {
      'aria-invalid': errosCampo[campo] ? 'true' : undefined,
      'aria-describedby': errosCampo[campo] ? `plataforma-erro-${campo}` : undefined,
    };
  }

  function cancelarEdicao() {
    setIdEdicao(null);
    setModelo(vazio);
    setErro('');
    setErrosCampo({});
  }

  return (
    <>
      <h1 className="h2">Plataformas</h1>
      <p className="text-secondary">Cadastre e mantenha os consoles disponíveis.</p>
      {erro && <MensagemErro mensagem={erro} aoTentarNovamente={carregar} />}

      <form className="card rg-painel rg-formulario mb-4" onSubmit={salvar} noValidate ref={formularioRef}>
        <div className="card-body p-3 p-md-4">
          <h2 className="h5 mb-3">{idEdicao ? 'Editar plataforma' : 'Nova plataforma'}</h2>
          <div className="row g-3">
            <div className="col-12 col-md-5">
              <label className="form-label" htmlFor="plataforma-nome">Nome da plataforma</label>
              <input
                id="plataforma-nome"
                className={`form-control${classeInvalida('nome')}`}
                placeholder="Nome"
                required
                maxLength="80"
                value={modelo.nome}
                onChange={(evento) => alterar('nome', evento.target.value)}
                {...ligacao('nome')}
              />
              {mensagensDoCampo('nome')}
            </div>
            <div className="col-12 col-sm-8 col-md-4">
              <label className="form-label" htmlFor="plataforma-fabricante">Fabricante</label>
              <input
                id="plataforma-fabricante"
                className={`form-control${classeInvalida('fabricante')}`}
                placeholder="Fabricante"
                required
                maxLength="80"
                value={modelo.fabricante}
                onChange={(evento) => alterar('fabricante', evento.target.value)}
                {...ligacao('fabricante')}
              />
              {mensagensDoCampo('fabricante')}
            </div>
            <div className="col-12 col-sm-4 col-md-3">
              <label className="form-label" htmlFor="plataforma-ano">Ano de lançamento</label>
              <input
                id="plataforma-ano"
                className={`form-control${classeInvalida('anoLancamento')}`}
                type="number"
                inputMode="numeric"
                min="1970"
                max="2100"
                required
                value={modelo.anoLancamento}
                onChange={(evento) => alterar('anoLancamento', Number(evento.target.value))}
                {...ligacao('anoLancamento')}
              />
              {mensagensDoCampo('anoLancamento')}
            </div>
          </div>
        </div>
        <div className="card-footer p-3 p-md-4 d-grid d-sm-flex justify-content-sm-end gap-2">
          {idEdicao && (
            <button type="button" className="btn btn-outline-dark" onClick={cancelarEdicao}>
              Cancelar edição
            </button>
          )}
          <button className="btn btn-retro">Salvar</button>
        </div>
      </form>

      {!lista && !erro ? (
        <Carregando />
      ) : lista?.length ? (
        <div className="rg-painel rg-lista-admin overflow-hidden">
          <ul className="list-unstyled mb-0">
            {lista.map((plataforma) => (
              <li
                className={`rg-item-admin d-flex flex-column flex-sm-row align-items-sm-center justify-content-between gap-3${idEdicao === plataforma.id ? ' rg-item-em-edicao' : ''}`}
                key={plataforma.id}
              >
                <div>
                  <strong>{plataforma.nome}</strong>
                  {idEdicao === plataforma.id && <span className="rg-situacao rg-situacao-estoque ms-2">Em edição</span>}
                  <div className="small text-secondary">
                    {plataforma.fabricante} · {plataforma.anoLancamento}
                  </div>
                </div>
                <div className="rg-acoes">
                  <button
                    className="btn btn-sm btn-outline-dark"
                    type="button"
                    aria-label={`Editar ${plataforma.nome}`}
                    onClick={() => editar(plataforma)}
                  >
                    Editar
                  </button>
                  <button
                    className="btn btn-sm btn-outline-danger"
                    type="button"
                    aria-label={`Excluir ${plataforma.nome}`}
                    onClick={() => excluir(plataforma.id)}
                  >
                    Excluir
                  </button>
                </div>
              </li>
            ))}
          </ul>
        </div>
      ) : !erro ? (
        <EstadoVazio titulo="Nenhuma plataforma cadastrada">
          Adicione a primeira plataforma no formulário.
        </EstadoVazio>
      ) : null}
    </>
  );
}
