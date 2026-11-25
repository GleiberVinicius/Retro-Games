import { Link } from 'react-router-dom';
import { formatarPreco } from '../lib/formato';

// Cor decorativa do cartão, escolhida pelo nome da plataforma (sempre a mesma para a mesma plataforma).
function indiceDeCor(texto = '') {
  let valor = 0;
  for (const letra of texto) valor = (valor * 31 + letra.charCodeAt(0)) >>> 0;
  return valor % 6;
}

export default function CardJogo({ jogo }) {
  const cor = `rg-cor-${indiceDeCor(jogo.plataforma)}`;
  const esgotado = !(jogo.estoque > 0);

  return (
    <article className={`card rg-painel rg-card-jogo ${cor.replace('cor', 'fundo')}`}>
      <div className={`rg-card-topo ${cor}`} aria-hidden="true" />
      <div className="card-body d-flex flex-column">
        <div className="d-flex justify-content-between align-items-center gap-2 mb-2">
          <span className={`badge etiqueta ${cor}`}>{jogo.plataforma}</span>
          <span className="text-secondary small">{jogo.anoLancamento}</span>
        </div>
        <h2 className="h5">{jogo.nome}</h2>
        <p className="small text-secondary mb-2">{jogo.condicao}</p>
        <div className="d-flex flex-wrap gap-1 mb-3">
          {jogo.generos.map((genero) => <span className="rg-chip" key={genero}>{genero}</span>)}
        </div>
        <div className="mt-auto">
          <div className="d-flex justify-content-between align-items-end gap-2 flex-wrap mb-3">
            <div className={`rg-preco${esgotado ? ' rg-preco-esgotado' : ''}`}>{formatarPreco(jogo.preco)}</div>
            <span className={`rg-situacao ${esgotado ? 'rg-situacao-esgotado' : 'rg-situacao-estoque'}`}>
              {esgotado ? 'Esgotado' : `${jogo.estoque} em estoque`}
            </span>
          </div>
          <Link className="btn btn-outline-dark w-100" to={`/jogos/${jogo.id}`} aria-label={`Ver jogo ${jogo.nome}`}>Ver jogo</Link>
        </div>
      </div>
    </article>
  );
}
