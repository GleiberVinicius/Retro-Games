import { useCallback, useEffect, useState } from 'react';
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
    return errosCampo[campo]?.map((mensagem) => (
      <div className="invalid-feedback" key={mensagem}>{mensagem}</div>
    ));
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

      <form className="card border-0 shadow-sm mb-4" onSubmit={salvar} noValidate>
        <div className="card-body">
          <h2 className="h5">{idEdicao ? 'Editar plataforma' : 'Nova plataforma'}</h2>
          <div className="row g-2">
            <div className="col-md-5">
              <input
                className={`form-control${classeInvalida('nome')}`}
                aria-label="Nome da plataforma"
                placeholder="Nome"
                required
                maxLength="80"
                value={modelo.nome}
                onChange={(evento) => alterar('nome', evento.target.value)}
              />
              {mensagensDoCampo('nome')}
            </div>
            <div className="col-md-4">
              <input
                className={`form-control${classeInvalida('fabricante')}`}
                aria-label="Fabricante"
                placeholder="Fabricante"
                required
                maxLength="80"
                value={modelo.fabricante}
                onChange={(evento) => alterar('fabricante', evento.target.value)}
              />
              {mensagensDoCampo('fabricante')}
            </div>
            <div className="col-md-2">
              <input
                className={`form-control${classeInvalida('anoLancamento')}`}
                aria-label="Ano de lançamento"
                type="number"
                min="1970"
                max="2100"
                required
                value={modelo.anoLancamento}
                onChange={(evento) => alterar('anoLancamento', Number(evento.target.value))}
              />
              {mensagensDoCampo('anoLancamento')}
            </div>
            <div className="col-md-1 d-grid">
              <button className="btn btn-dark">Salvar</button>
            </div>
          </div>
        </div>
        {idEdicao && (
          <div className="card-footer bg-white">
            <button type="button" className="btn btn-sm btn-link" onClick={cancelarEdicao}>
              Cancelar edição
            </button>
          </div>
        )}
      </form>

      {!lista && !erro ? (
        <Carregando />
      ) : lista?.length ? (
        <div className="list-group">
          {lista.map((plataforma) => (
            <div
              className="list-group-item d-flex align-items-center justify-content-between"
              key={plataforma.id}
            >
              <div>
                <strong>{plataforma.nome}</strong>
                <div className="small text-secondary">
                  {plataforma.fabricante} · {plataforma.anoLancamento}
                </div>
              </div>
              <div>
                <button
                  className="btn btn-sm btn-outline-dark me-2"
                  onClick={() => editar(plataforma)}
                >
                  Editar
                </button>
                <button
                  className="btn btn-sm btn-outline-danger"
                  onClick={() => excluir(plataforma.id)}
                >
                  Excluir
                </button>
              </div>
            </div>
          ))}
        </div>
      ) : !erro ? (
        <EstadoVazio titulo="Nenhuma plataforma cadastrada">
          Adicione a primeira plataforma no formulário.
        </EstadoVazio>
      ) : null}
    </>
  );
}
