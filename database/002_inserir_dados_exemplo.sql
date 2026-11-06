USE retrogames;

-- Ajusta as condições inicialmente criadas pelo script 001.
UPDATE tb_condicao_jogo
SET ds_condicao_jogo = 'Usado'
WHERE co_condicao_jogo = 2;

UPDATE tb_condicao_jogo
SET ds_condicao_jogo = 'Recondicionado'
WHERE co_condicao_jogo = 3;

DELETE FROM tb_condicao_jogo
WHERE co_condicao_jogo = 4;

-- Completa o catálogo de gêneros sem duplicar nomes ao reexecutar o script.
INSERT INTO tb_genero (no_genero)
VALUES ('Corrida'), ('Luta'), ('Quebra-cabeça')
ON DUPLICATE KEY UPDATE no_genero = VALUES(no_genero);

-- Plataformas do catálogo. O nome é único e permite reexecução segura.
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
  ('Atari 2600', 'Atari', 1977),
  ('Nintendo Entertainment System', 'Nintendo', 1983),
  ('Super Nintendo Entertainment System', 'Nintendo', 1990),
  ('Mega Drive', 'SEGA', 1988),
  ('Game Boy', 'Nintendo', 1989),
  ('PlayStation', 'Sony', 1994)
ON DUPLICATE KEY UPDATE
  no_fabricante = VALUES(no_fabricante),
  nu_ano_lancamento = VALUES(nu_ano_lancamento);

-- Inclui cada jogo somente se ainda não existir na mesma plataforma.
INSERT INTO tb_jogo (
  no_jogo,
  ds_jogo,
  nu_ano_lancamento,
  vl_preco,
  qt_estoque,
  st_ativo,
  dt_cadastro,
  co_plataforma,
  co_condicao_jogo
)
SELECT
  dados.no_jogo,
  dados.ds_jogo,
  dados.nu_ano_lancamento,
  dados.vl_preco,
  dados.qt_estoque,
  dados.st_ativo,
  dados.dt_cadastro,
  plataforma.co_seq_plataforma,
  1
FROM (
  SELECT 'Adventure' AS no_jogo, 'Aventura em mundo aberto para Atari 2600.' AS ds_jogo, 1980 AS nu_ano_lancamento, 89.90 AS vl_preco, 5 AS qt_estoque, TRUE AS st_ativo, '2026-11-06 12:00:00' AS dt_cadastro, 'Atari 2600' AS no_plataforma
  UNION ALL SELECT 'Pitfall!', 'Aventura de exploração e plataforma.', 1982, 79.90, 4, TRUE, '2026-11-06 12:00:00', 'Atari 2600'
  UNION ALL SELECT 'Pac-Man', 'Clássico jogo de labirinto.', 1982, 69.90, 6, TRUE, '2026-11-06 12:00:00', 'Atari 2600'
  UNION ALL SELECT 'E.T. the Extra-Terrestrial', 'Adaptação de aventura para Atari 2600.', 1982, 29.90, 2, FALSE, '2026-11-06 12:00:00', 'Atari 2600'
  UNION ALL SELECT 'Super Mario Bros.', 'Aventura de plataforma do encanador Mario.', 1985, 99.90, 8, TRUE, '2026-11-06 12:00:00', 'Nintendo Entertainment System'
  UNION ALL SELECT 'The Legend of Zelda', 'Aventura de exploração no reino de Hyrule.', 1986, 129.90, 3, TRUE, '2026-11-06 12:00:00', 'Nintendo Entertainment System'
  UNION ALL SELECT 'Metroid', 'Aventura de exploração espacial.', 1986, 109.90, 3, TRUE, '2026-11-06 12:00:00', 'Nintendo Entertainment System'
  UNION ALL SELECT 'Castlevania', 'Aventura de ação contra criaturas sobrenaturais.', 1986, 119.90, 4, TRUE, '2026-11-06 12:00:00', 'Nintendo Entertainment System'
  UNION ALL SELECT 'Super Mario World', 'Aventura de plataforma para Super Nintendo.', 1990, 149.90, 7, TRUE, '2026-11-06 12:00:00', 'Super Nintendo Entertainment System'
  UNION ALL SELECT 'The Legend of Zelda: A Link to the Past', 'Aventura e RPG no reino de Hyrule.', 1991, 169.90, 4, TRUE, '2026-11-06 12:00:00', 'Super Nintendo Entertainment System'
  UNION ALL SELECT 'Super Metroid', 'Aventura de exploração espacial.', 1994, 179.90, 3, TRUE, '2026-11-06 12:00:00', 'Super Nintendo Entertainment System'
  UNION ALL SELECT 'Donkey Kong Country', 'Aventura de plataforma com Donkey Kong.', 1994, 159.90, 5, TRUE, '2026-11-06 12:00:00', 'Super Nintendo Entertainment System'
  UNION ALL SELECT 'Golden Axe', 'Aventura de ação e combate fantástico.', 1989, 119.90, 3, TRUE, '2026-11-06 12:00:00', 'Mega Drive'
  UNION ALL SELECT 'Sonic the Hedgehog', 'Aventura de plataforma em alta velocidade.', 1991, 129.90, 6, TRUE, '2026-11-06 12:00:00', 'Mega Drive'
  UNION ALL SELECT 'Streets of Rage 2', 'Aventura de combate nas ruas da cidade.', 1992, 139.90, 4, TRUE, '2026-11-06 12:00:00', 'Mega Drive'
  UNION ALL SELECT 'Tetris', 'Quebra-cabeça de blocos para Game Boy.', 1989, 59.90, 0, TRUE, '2026-11-06 12:00:00', 'Game Boy'
  UNION ALL SELECT 'Super Mario Land', 'Aventura de plataforma portátil do Mario.', 1989, 89.90, 5, TRUE, '2026-11-06 12:00:00', 'Game Boy'
  UNION ALL SELECT 'Pokémon Red Version', 'Aventura de captura e treinamento de criaturas.', 1996, 149.90, 4, TRUE, '2026-11-06 12:00:00', 'Game Boy'
  UNION ALL SELECT 'Crash Bandicoot', 'Aventura de plataforma em 3D.', 1996, 179.90, 5, TRUE, '2026-11-06 12:00:00', 'PlayStation'
  UNION ALL SELECT 'Gran Turismo', 'Simulador de corridas para PlayStation.', 1997, 189.90, 3, TRUE, '2026-11-06 12:00:00', 'PlayStation'
) AS dados
INNER JOIN tb_plataforma AS plataforma
  ON plataforma.no_plataforma = dados.no_plataforma
WHERE NOT EXISTS (
  SELECT 1
  FROM tb_jogo AS jogo_existente
  WHERE jogo_existente.no_jogo = dados.no_jogo
    AND jogo_existente.co_plataforma = plataforma.co_seq_plataforma
);

-- Associa três gêneros a cada um dos vinte jogos.
INSERT IGNORE INTO tb_jogo_genero (co_jogo, co_genero)
SELECT jogo.co_seq_jogo, genero.co_seq_genero
FROM (
  SELECT 'Adventure' AS no_jogo, 'Atari 2600' AS no_plataforma, 'Ação' AS no_genero
  UNION ALL SELECT 'Adventure', 'Atari 2600', 'Aventura'
  UNION ALL SELECT 'Adventure', 'Atari 2600', 'Quebra-cabeça'
  UNION ALL SELECT 'Pitfall!', 'Atari 2600', 'Ação'
  UNION ALL SELECT 'Pitfall!', 'Atari 2600', 'Aventura'
  UNION ALL SELECT 'Pitfall!', 'Atari 2600', 'Plataforma'
  UNION ALL SELECT 'Pac-Man', 'Atari 2600', 'Ação'
  UNION ALL SELECT 'Pac-Man', 'Atari 2600', 'Aventura'
  UNION ALL SELECT 'Pac-Man', 'Atari 2600', 'Quebra-cabeça'
  UNION ALL SELECT 'E.T. the Extra-Terrestrial', 'Atari 2600', 'Ação'
  UNION ALL SELECT 'E.T. the Extra-Terrestrial', 'Atari 2600', 'Aventura'
  UNION ALL SELECT 'E.T. the Extra-Terrestrial', 'Atari 2600', 'Quebra-cabeça'
  UNION ALL SELECT 'Super Mario Bros.', 'Nintendo Entertainment System', 'Ação'
  UNION ALL SELECT 'Super Mario Bros.', 'Nintendo Entertainment System', 'Aventura'
  UNION ALL SELECT 'Super Mario Bros.', 'Nintendo Entertainment System', 'Plataforma'
  UNION ALL SELECT 'The Legend of Zelda', 'Nintendo Entertainment System', 'Ação'
  UNION ALL SELECT 'The Legend of Zelda', 'Nintendo Entertainment System', 'Aventura'
  UNION ALL SELECT 'The Legend of Zelda', 'Nintendo Entertainment System', 'RPG'
  UNION ALL SELECT 'Metroid', 'Nintendo Entertainment System', 'Ação'
  UNION ALL SELECT 'Metroid', 'Nintendo Entertainment System', 'Aventura'
  UNION ALL SELECT 'Metroid', 'Nintendo Entertainment System', 'Plataforma'
  UNION ALL SELECT 'Castlevania', 'Nintendo Entertainment System', 'Ação'
  UNION ALL SELECT 'Castlevania', 'Nintendo Entertainment System', 'Aventura'
  UNION ALL SELECT 'Castlevania', 'Nintendo Entertainment System', 'Plataforma'
  UNION ALL SELECT 'Super Mario World', 'Super Nintendo Entertainment System', 'Ação'
  UNION ALL SELECT 'Super Mario World', 'Super Nintendo Entertainment System', 'Aventura'
  UNION ALL SELECT 'Super Mario World', 'Super Nintendo Entertainment System', 'Plataforma'
  UNION ALL SELECT 'The Legend of Zelda: A Link to the Past', 'Super Nintendo Entertainment System', 'Ação'
  UNION ALL SELECT 'The Legend of Zelda: A Link to the Past', 'Super Nintendo Entertainment System', 'Aventura'
  UNION ALL SELECT 'The Legend of Zelda: A Link to the Past', 'Super Nintendo Entertainment System', 'RPG'
  UNION ALL SELECT 'Super Metroid', 'Super Nintendo Entertainment System', 'Ação'
  UNION ALL SELECT 'Super Metroid', 'Super Nintendo Entertainment System', 'Aventura'
  UNION ALL SELECT 'Super Metroid', 'Super Nintendo Entertainment System', 'Plataforma'
  UNION ALL SELECT 'Donkey Kong Country', 'Super Nintendo Entertainment System', 'Ação'
  UNION ALL SELECT 'Donkey Kong Country', 'Super Nintendo Entertainment System', 'Aventura'
  UNION ALL SELECT 'Donkey Kong Country', 'Super Nintendo Entertainment System', 'Plataforma'
  UNION ALL SELECT 'Golden Axe', 'Mega Drive', 'Ação'
  UNION ALL SELECT 'Golden Axe', 'Mega Drive', 'Aventura'
  UNION ALL SELECT 'Golden Axe', 'Mega Drive', 'Luta'
  UNION ALL SELECT 'Sonic the Hedgehog', 'Mega Drive', 'Ação'
  UNION ALL SELECT 'Sonic the Hedgehog', 'Mega Drive', 'Aventura'
  UNION ALL SELECT 'Sonic the Hedgehog', 'Mega Drive', 'Plataforma'
  UNION ALL SELECT 'Streets of Rage 2', 'Mega Drive', 'Ação'
  UNION ALL SELECT 'Streets of Rage 2', 'Mega Drive', 'Aventura'
  UNION ALL SELECT 'Streets of Rage 2', 'Mega Drive', 'Luta'
  UNION ALL SELECT 'Tetris', 'Game Boy', 'Ação'
  UNION ALL SELECT 'Tetris', 'Game Boy', 'Aventura'
  UNION ALL SELECT 'Tetris', 'Game Boy', 'Quebra-cabeça'
  UNION ALL SELECT 'Super Mario Land', 'Game Boy', 'Ação'
  UNION ALL SELECT 'Super Mario Land', 'Game Boy', 'Aventura'
  UNION ALL SELECT 'Super Mario Land', 'Game Boy', 'Plataforma'
  UNION ALL SELECT 'Pokémon Red Version', 'Game Boy', 'Ação'
  UNION ALL SELECT 'Pokémon Red Version', 'Game Boy', 'Aventura'
  UNION ALL SELECT 'Pokémon Red Version', 'Game Boy', 'RPG'
  UNION ALL SELECT 'Crash Bandicoot', 'PlayStation', 'Ação'
  UNION ALL SELECT 'Crash Bandicoot', 'PlayStation', 'Aventura'
  UNION ALL SELECT 'Crash Bandicoot', 'PlayStation', 'Plataforma'
  UNION ALL SELECT 'Gran Turismo', 'PlayStation', 'Ação'
  UNION ALL SELECT 'Gran Turismo', 'PlayStation', 'Corrida'
  UNION ALL SELECT 'Gran Turismo', 'PlayStation', 'Esporte'
) AS associacoes
INNER JOIN tb_plataforma AS plataforma
  ON plataforma.no_plataforma = associacoes.no_plataforma
INNER JOIN tb_jogo AS jogo
  ON jogo.no_jogo = associacoes.no_jogo
 AND jogo.co_plataforma = plataforma.co_seq_plataforma
INNER JOIN tb_genero AS genero
  ON genero.no_genero = associacoes.no_genero;

-- Consultas de conferência para executar após os INSERTs.
SELECT COUNT(*) AS total_plataformas FROM tb_plataforma;
SELECT COUNT(*) AS total_generos FROM tb_genero;
SELECT COUNT(*) AS total_jogos FROM tb_jogo;
SELECT jogo.no_jogo, COUNT(*) AS total_generos
FROM tb_jogo AS jogo
LEFT JOIN tb_jogo_genero AS jogo_genero ON jogo_genero.co_jogo = jogo.co_seq_jogo
GROUP BY jogo.co_seq_jogo, jogo.no_jogo
ORDER BY jogo.no_jogo;
SELECT no_jogo, st_ativo, qt_estoque
FROM tb_jogo
WHERE st_ativo = FALSE OR qt_estoque = 0;
SELECT COUNT(*) AS total_usuarios FROM tb_usuario;
SELECT co_condicao_jogo, ds_condicao_jogo FROM tb_condicao_jogo ORDER BY co_condicao_jogo;
SELECT co_perfil, ds_perfil FROM tb_perfil ORDER BY co_perfil;
