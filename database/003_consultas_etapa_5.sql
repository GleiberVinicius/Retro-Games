USE retrogames;

-- 1. Catálogo público: somente jogos ativos, com plataforma e condição,
-- ordenados pelo nome do jogo. O jogo inativo E.T. não deve aparecer.
SELECT
  jogo.no_jogo,
  plataforma.no_plataforma,
  condicao.ds_condicao_jogo,
  jogo.ds_jogo,
  jogo.nu_ano_lancamento,
  jogo.vl_preco,
  jogo.qt_estoque
FROM tb_jogo AS jogo
INNER JOIN tb_plataforma AS plataforma
  ON plataforma.co_seq_plataforma = jogo.co_plataforma
INNER JOIN tb_condicao_jogo AS condicao
  ON condicao.co_condicao_jogo = jogo.co_condicao_jogo
WHERE jogo.st_ativo = TRUE
ORDER BY jogo.no_jogo;

-- 2. Gêneros do jogo informado. Troque o texto abaixo para consultar outro.
SET @nome_jogo = 'The Legend of Zelda';

SELECT jogo.no_jogo, genero.no_genero
FROM tb_jogo AS jogo
INNER JOIN tb_jogo_genero AS jogo_genero
  ON jogo_genero.co_jogo = jogo.co_seq_jogo
INNER JOIN tb_genero AS genero
  ON genero.co_seq_genero = jogo_genero.co_genero
WHERE jogo.no_jogo = @nome_jogo
ORDER BY genero.no_genero;

-- 3. Jogos associados ao gênero informado. Troque o texto abaixo para consultar outro.
SET @nome_genero = 'Aventura';

SELECT genero.no_genero, jogo.no_jogo, plataforma.no_plataforma
FROM tb_genero AS genero
INNER JOIN tb_jogo_genero AS jogo_genero
  ON jogo_genero.co_genero = genero.co_seq_genero
INNER JOIN tb_jogo AS jogo
  ON jogo.co_seq_jogo = jogo_genero.co_jogo
INNER JOIN tb_plataforma AS plataforma
  ON plataforma.co_seq_plataforma = jogo.co_plataforma
WHERE genero.no_genero = @nome_genero
ORDER BY jogo.no_jogo;

-- 4. Jogos por plataforma, incluindo plataformas sem jogos.
-- Esta plataforma temporária deve aparecer com qt_jogos = 0.
INSERT IGNORE INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('__teste_etapa5_sem_jogos__', 'RetroGames', 2026);

SELECT plataforma.no_plataforma, COUNT(jogo.co_seq_jogo) AS qt_jogos
FROM tb_plataforma AS plataforma
LEFT JOIN tb_jogo AS jogo
  ON jogo.co_plataforma = plataforma.co_seq_plataforma
GROUP BY plataforma.co_seq_plataforma, plataforma.no_plataforma
ORDER BY plataforma.no_plataforma;

-- Limpa a plataforma temporária criada para a consulta 4.
DELETE FROM tb_plataforma
WHERE co_seq_plataforma > 0
  AND no_plataforma = '__teste_etapa5_sem_jogos__';

-- 5. Busca por parte do nome na página 2, com 5 itens por página.
-- O trecho 'e' encontra mais de 5 jogos ativos do catálogo de exemplo.
SET @parte_nome = 'e';

SELECT no_jogo
FROM tb_jogo
WHERE st_ativo = TRUE
  AND no_jogo LIKE CONCAT('%', @parte_nome, '%')
ORDER BY no_jogo
LIMIT 5 OFFSET 5;
