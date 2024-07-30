using Newtonsoft.Json;
using Spectre.Console;
using System.Diagnostics;


class Program
{
    static void Main(string[] args)
    {
        // Stampa il titolo dell'applicazione
        AnsiConsole.MarkupLine("[bold green]Gestionale delle Prodotti Aziendali[/]");

        string pathProdotti = @"GestioneProdotti.json";
        string pathCategorie = @"Categorie.json";

        // Carica i dati dei prodotti e delle categorie
        List<dynamic> Prodotti = CaricaProdotti(pathProdotti);
        List<string> Categorie = CaricaCategorie(pathCategorie);

        while (true)
        {
            var menu = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Scegli un'opzione:")
                    .AddChoices("Gestione Prodotti", "Categorie di Prodotto", "Report e Analisi", "Ricerca Prodotto", "Esci"));

            switch (menu)
            {
                case "Gestione Prodotti":
                    GestioneProdotti(Prodotti, Categorie, pathProdotti, pathCategorie);
                    break;

                case "Categorie di Prodotto":
                    VisualizzaCategorie(Prodotti, Categorie);
                    break;

                case "Report e Analisi":
                    var reportMenu = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Scegli un report:")
                            .AddChoices("Vendite Mensili", "Vendite per Prodotto",
                                        "Fasce di Prezzo", "Giorni della Settimana", "KPI", "Torna al Menù Principale"));

                    switch (reportMenu)
                    {
                        case "Vendite Mensili":
                            ReportVenditeMese(Prodotti);
                            break;

                        case "Vendite per Prodotto":
                            ReportVenditeProdotto(Prodotti);
                            break;

                        case "Fasce di Prezzo":
                            ReportVenditeFascePrezzo(Prodotti);
                            break;

                        case "Giorni della Settimana":
                            ReportVenditeGiorni(Prodotti);
                            break;
                        case "KPI":
                            /*ReportKPI(Prodotti);*/
                            break;
                    }
                    break;

                case "Ricerca Prodotto":
                    RicercaProdotti(Prodotti);
                    break;

                case "Esci":
                    return;
            }
        }
    }

    static List<dynamic> CaricaProdotti(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<dynamic>>(json)!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il caricamento dei prodotti: {ex.Message}");
                return new List<dynamic>();
            }
        }
        else
        {
            return new List<dynamic>();
        }
    }

    static List<string> CaricaCategorie(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<string>>(json)!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il caricamento delle categorie: {ex.Message}");
                return new List<string>();
            }
        }
        else
        {
            return new List<string>();
        }
    }

    static void GestioneProdotti(List<dynamic> Prodotti, List<string> Categorie, string pathProdotti, string pathCategorie)
    {
        while (true)
        {
            var sottoMenu = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Scegli un'azione:")
                    .AddChoices("Aggiungi Prodotto", "Visualizza Prodotti", "Modifica Prodotto", "Elimina Prodotto", "Torna al Menù Principale"));

            switch (sottoMenu)
            {
                case "Aggiungi Prodotto":
                    AggiungiProdotto(Prodotti, Categorie, pathProdotti, pathCategorie);
                    break;

                case "Visualizza Prodotti":
                    VisualizzaProdotti(Prodotti);
                    break;

                case "Modifica Prodotto":
                    ModificaProdotto(Prodotti, Categorie, pathProdotti, pathCategorie);
                    break;

                case "Elimina Prodotto":
                    EliminaProdotto(Prodotti, Categorie, pathProdotti, pathCategorie);
                    break;

                case "Torna al Menù Principale":
                    return;
            }
        }
    }
    static void AggiungiProdotto(List<dynamic> Prodotti, List<string> Categorie, string pathProdotti, string pathCategorie)
    {
        DateTime data = DateTime.Now;
        string prodotto = AnsiConsole.Prompt(new TextPrompt<string>("Inserisci il nome del prodotto:"));
        decimal importo = AnsiConsole.Prompt(new TextPrompt<decimal>("Inserisci l'importo del prodotto:"));
        string categoria = AnsiConsole.Prompt(new TextPrompt<string>("Inserisci la categoria:"));
        string descrizione = AnsiConsole.Prompt(new TextPrompt<string>("Inserisci una descrizione per il prodotto:"));

        var nuovoProdotto = new
        {
            Data = data,
            Importo = importo,
            Prodotto = prodotto,
            Categoria = categoria,
            Descrizione = descrizione
        };

        Prodotti.Add(nuovoProdotto);

        // Verifica e aggiungi la categoria se non esiste già
        if (!Categorie.Contains(categoria))
        {
            Categorie.Add(categoria);
            string jsonCategorie = JsonConvert.SerializeObject(Categorie, Formatting.Indented);
            File.WriteAllText(pathCategorie, jsonCategorie);
        }

        string jsonProdotti = JsonConvert.SerializeObject(Prodotti, Formatting.Indented);
        File.WriteAllText(pathProdotti, jsonProdotti);

        AnsiConsole.MarkupLine("[green]Prodotto aggiunto con successo![/]");
    }

    static void VisualizzaProdotti(List<dynamic> Prodotti)
    {
        if (Prodotti.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nessun prodotto registrato.[/]");
            return;
        }

        var criterio = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Scegli l'ordine di visualizzazione:")
                .AddChoices("Alfabetico (crescente)", "Alfabetico (decrescente)",
                            "Data (crescente)", "Data (decrescente)",
                            "Categoria (crescente)", "Categoria (decrescente)",
                            "Prezzo (alto a basso)", "Prezzo (basso ad alto)"));

        List<dynamic> prodottiOrdinati = new List<dynamic>(Prodotti);

        switch (criterio)
        {
            case "Alfabetico (crescente)":
                prodottiOrdinati.Sort(ConfrontoNomeCrescente);
                break;
            case "Alfabetico (decrescente)":
                prodottiOrdinati.Sort(ConfrontoNomeDecrescente);
                break;
            case "Data (crescente)":
                prodottiOrdinati.Sort(ConfrontoDataCrescente);
                break;
            case "Data (decrescente)":
                prodottiOrdinati.Sort(ConfrontoDataDecrescente);
                break;
            case "Categoria (crescente)":
                prodottiOrdinati.Sort(ConfrontoCategoriaCrescente);
                break;
            case "Categoria (decrescente)":
                prodottiOrdinati.Sort(ConfrontoCategoriaDecrescente);
                break;
            case "Prezzo (alto a basso)":
                prodottiOrdinati.Sort(ConfrontoPrezzoDecrescente);
                break;
            case "Prezzo (basso ad alto)":
                prodottiOrdinati.Sort(ConfrontoPrezzoCrescente);
                break;
        }

        VisualizzaProdottiInTabella(prodottiOrdinati);

        var esporta = AnsiConsole.Prompt(
           new SelectionPrompt<string>()
               .Title("Vuoi esportare in csv? (excel)")
               .AddChoices("si", "no"));

        if (esporta == "si")
        {
            EsportaCsvProd(prodottiOrdinati);
        }
    }

    static void VisualizzaProdottiInTabella(List<dynamic> prodottiOrdinati)
    {
        var tabella = new Table();
        tabella.AddColumn("Indice");
        tabella.AddColumn("Data");
        tabella.AddColumn("Orario");
        tabella.AddColumn("Importo");
        tabella.AddColumn("Prodotto");
        tabella.AddColumn("Categoria");
        tabella.AddColumn("Descrizione");

        decimal totaleImporti = 0;

        for (int i = 0; i < prodottiOrdinati.Count; i++)
        {
            var prodottoItem = prodottiOrdinati[i];
            string orario = ((DateTime)prodottoItem.Data).ToString("HH:mm");
            decimal importo = (decimal)prodottoItem.Importo;
            totaleImporti += importo;
            tabella.AddRow(i.ToString(), ((DateTime)prodottoItem.Data).ToShortDateString(), orario, importo.ToString("C"), (string)prodottoItem.Prodotto, (string)prodottoItem.Categoria, (string)prodottoItem.Descrizione);
        }

        var panel = new Panel(tabella)
        {
            Header = new PanelHeader("[bold blue]Dettagli Prodotti[/]")
        };

        AnsiConsole.Write(panel);

        // Visualizza il totale
        AnsiConsole.MarkupLine($"[bold cyan]Totale Importi: {totaleImporti:C}[/]");
    }

    static void ModificaProdotto(List<dynamic> Prodotti, List<string> Categorie, string pathProdotti, string pathCategorie)
    {
        if (Prodotti.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nessun prodotto registrato.[/]");
            return;
        }

        VisualizzaProdottiInTabella(Prodotti);

        int index;
        while (true)
        {
            index = AnsiConsole.Prompt(new TextPrompt<int>("Inserisci l'indice del prodotto da modificare:"));
            if (index >= 0 && index < Prodotti.Count)
            {
                break;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Indice non valido. Riprovare.[/]");
            }
        }

        var prodottoDaModificare = Prodotti[index];

        var campoDaModificare = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Scegli il campo da modificare:")
                .AddChoices("Data", "Importo", "Prodotto", "Categoria", "Descrizione"));

        switch (campoDaModificare)
        {
            case "Data":
                prodottoDaModificare.Data = AnsiConsole.Prompt(new TextPrompt<DateTime>("Inserisci la nuova data (YYYY-MM-DD):"));
                break;
            case "Importo":
                prodottoDaModificare.Importo = AnsiConsole.Prompt(new TextPrompt<decimal>("Inserisci il nuovo importo:"));
                break;
            case "Prodotto":
                prodottoDaModificare.Prodotto = AnsiConsole.Prompt(new TextPrompt<string>("Inserisci il nuovo nome del prodotto:"));
                break;
            case "Categoria":
                var nuovaCategoria = AnsiConsole.Prompt(new TextPrompt<string>("Inserisci la nuova categoria:"));
                var vecchiaCategoria = (string)prodottoDaModificare.Categoria;
                prodottoDaModificare.Categoria = nuovaCategoria;

                // Aggiorna la lista delle categorie
                if (!Categorie.Contains(nuovaCategoria))
                {
                    Categorie.Add(nuovaCategoria);
                }

                // Rimuovi la vecchia categoria se non ci sono più prodotti associati
                bool categoriaPresente = false;
                foreach (var prodotto in Prodotti)
                {
                    if ((string)prodotto.Categoria == vecchiaCategoria)
                    {
                        categoriaPresente = true;
                        break;
                    }
                }

                if (!categoriaPresente)
                {
                    Categorie.Remove(vecchiaCategoria);
                }

                // Salva le categorie aggiornate
                string jsonCategorie = JsonConvert.SerializeObject(Categorie, Formatting.Indented);
                File.WriteAllText(pathCategorie, jsonCategorie);
                break;
            case "Descrizione":
                prodottoDaModificare.Descrizione = AnsiConsole.Prompt(new TextPrompt<string>("Inserisci una nuova descrizione:"));
                break;
        }

        // Salva i prodotti aggiornati nel file
        string jsonProdotti = JsonConvert.SerializeObject(Prodotti, Formatting.Indented);
        File.WriteAllText(pathProdotti, jsonProdotti);

        AnsiConsole.MarkupLine("[green]Prodotto modificato con successo![/]");
    }

    static void EliminaProdotto(List<dynamic> Prodotti, List<string> Categorie, string pathProdotti, string pathCategorie)
    {
        if (Prodotti.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nessun prodotto registrato.[/]");
            return;
        }

        VisualizzaProdottiInTabella(Prodotti);

        int index;
        while (true)
        {
            index = AnsiConsole.Prompt(new TextPrompt<int>("Inserisci l'indice del prodotto da eliminare:"));
            if (index >= 0 && index < Prodotti.Count)
            {
                break;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Indice non valido. Riprovare.[/]");
            }
        }

        var prodottoDaEliminare = Prodotti[index];
        var categoriaDaEliminare = (string)prodottoDaEliminare.Categoria;

        // Rimuove il prodotto
        Prodotti.RemoveAt(index);

        // Verifica se la categoria deve essere rimossa
        bool categoriaPresente = Prodotti.Any(p => (string)p.Categoria == categoriaDaEliminare);

        if (!categoriaPresente)
        {
            Categorie.Remove(categoriaDaEliminare);
        }

        // Salva i prodotti aggiornati nel file
        string jsonProdotti = JsonConvert.SerializeObject(Prodotti, Formatting.Indented);
        File.WriteAllText(pathProdotti, jsonProdotti);

        // Salva le categorie aggiornate nel file
        string jsonCategorie = JsonConvert.SerializeObject(Categorie, Formatting.Indented);
        File.WriteAllText(pathCategorie, jsonCategorie);

        AnsiConsole.MarkupLine("[red]Prodotto eliminato con successo![/]");
    }

    static void VisualizzaCategorie(List<dynamic> Prodotti, List<string> Categorie)
    {
        if (Categorie.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nessuna categoria registrata.[/]");
            return;
        }

        // Permetti la selezione multipla delle categorie
        var categorieSelezionate = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Seleziona le categorie:")
                .AddChoices(Categorie)
                .MoreChoicesText("[grey](Usa [green]spazio[grey] per selezionare, [green]Invio[grey] per confermare)[/]"));

        if (categorieSelezionate.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nessuna categoria selezionata.[/]");
            return;
        }

        // Filtra i prodotti in base alle categorie selezionate
        var prodottiFiltrati = new List<dynamic>();
        foreach (var prodotto in Prodotti)
        {
            if (categorieSelezionate.Contains((string)prodotto.Categoria))
            {
                prodottiFiltrati.Add(prodotto);
            }
        }

        if (prodottiFiltrati.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nessun prodotto trovato per le categorie selezionate.[/]");
        }
        else
        {
            VisualizzaProdottiInTabella(prodottiFiltrati);

            var esporta = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Vuoi esportare in csv? (excel)")
                    .AddChoices("si", "no"));

            if (esporta == "si")
            {
                foreach (var categoria in categorieSelezionate)
                {
                    EsportaCsvCat(Prodotti, categoria);
                }
            }
        }
    }


    static void EsportaCsvProd(List<dynamic> ProdottiOrdinati)
    {
        string filePath = @"prodotti.csv";
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Data,Importo,Prodotto,Categoria,Descrizione");
            foreach (var prodotto in ProdottiOrdinati)
            {
                string line = $"{((DateTime)prodotto.Data).ToShortDateString()},{((decimal)prodotto.Importo).ToString("F2")},{(string)prodotto.Prodotto},{(string)prodotto.Categoria},{(string)prodotto.Descrizione}";
                writer.WriteLine(line);
            }
        }
        AnsiConsole.MarkupLine($"[green]File CSV esportato con successo in {filePath}![/]");
    }
    static void EsportaCsvCat(List<dynamic> Prodotti, string categoriaSelezionata)
    {
        // Filtra i prodotti per la categoria selezionata
        var prodottiCategoria = new List<dynamic>();
        foreach (var prodotto in Prodotti)
        {
            if ((string)prodotto.Categoria == categoriaSelezionata)
            {
                prodottiCategoria.Add(prodotto);
            }
        }

        if (prodottiCategoria.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nessun prodotto trovato per la categoria selezionata.[/]");
            return;
        }

        // Percorso del file CSV specifico per la categoria
        string pathCSV2 = "ProdottiCategoria_" + categoriaSelezionata + ".csv";

        using (var writer = new StreamWriter(pathCSV2))
        {
            // Scrivi l'intestazione del file CSV
            writer.WriteLine("Indice,Data,Orario,Importo,Prodotto,Categoria,Descrizione");

            // Scrivi i dati dei prodotti della categoria scelta
            for (int i = 0; i < prodottiCategoria.Count; i++)
            {
                var prodottoItem = prodottiCategoria[i];
                string data = ((DateTime)prodottoItem.Data).ToShortDateString();
                string orario = ((DateTime)prodottoItem.Data).ToShortTimeString();
                writer.WriteLine($"{i},{data},{orario},{prodottoItem.Importo},{prodottoItem.Prodotto},{prodottoItem.Categoria},{prodottoItem.Descrizione}");
            }
        }

        AnsiConsole.MarkupLine("[green]Esportazione completata con successo![/]");
    }
    static void VisualizzaCat(List<dynamic> Prodotti, List<string> Categorie)
    {
        if (Categorie.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nessuna categoria registrata.[/]");
            return;
        }

        var categoriaSelezionata = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Categorie disponibili:")
                .AddChoices(Categorie));

        var prodottiFiltrati = new List<dynamic>();
        foreach (var prodotto in Prodotti)
        {
            if ((string)prodotto.Categoria == categoriaSelezionata)
            {
                prodottiFiltrati.Add(prodotto);
            }
        }

        if (prodottiFiltrati.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nessun prodotto trovato per la selezione della categoria.[/]");
        }
        else
        {
            VisualizzaProdottiInTabella(prodottiFiltrati);
            var esporta = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Vuoi esportare in csv? (excel)")
                    .AddChoices("si", "no"));

            if (esporta == "si")
            {
                EsportaCsvCat(Prodotti, categoriaSelezionata);
            }
        }
    }
    static void DisegnaGraficoBarre(string titolo, Dictionary<string, decimal> dati)
    {
        var tabella = new Table();
        tabella.AddColumn(titolo);
        tabella.AddColumn("Valore");

        foreach (var item in dati)
        {
            string etichetta = item.Key;
            decimal valore = item.Value;

            // Creazione della barra
            string barra = new string('█', (int)(valore / 10)); // Scalare per la visualizzazione

            tabella.AddRow(etichetta, $"{valore:C} {barra}");
        }

        AnsiConsole.Write(tabella);
    }
    static void ReportVenditeGiorni(List<dynamic> Prodotti)
    {
        decimal[] venditeGiorni = new decimal[7];
        string[] giorniSettimana =
        {
        "Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato"
    };

        // Calcola il totale delle vendite per ogni giorno della settimana
        foreach (var prodotto in Prodotti)
        {
            DateTime data = (DateTime)prodotto.Data;
            int giornoIndex = (int)data.DayOfWeek;
            venditeGiorni[giornoIndex] += (decimal)prodotto.Importo;
        }

        // Visualizza i risultati
        var tabella = new Table();
        tabella.AddColumn("Giorno");
        tabella.AddColumn("Totale");

        for (int i = 0; i < giorniSettimana.Length; i++)
        {
            tabella.AddRow(giorniSettimana[i], venditeGiorni[i].ToString("C"));
        }

        tabella.AddRow("[red]TOTALE DELLA SETTIMANA[/]", venditeGiorni.Sum().ToString("C"));

        AnsiConsole.Write(tabella);

        // Disegna il grafico a barre
        var datiGrafico = new Dictionary<string, decimal>();
        for (int i = 0; i < giorniSettimana.Length; i++)
        {
            datiGrafico[giorniSettimana[i]] = venditeGiorni[i];
        }
        DisegnaGraficoBarre("Vendite per Giorno della Settimana", datiGrafico);
    }
    static void ReportVenditeMese(List<dynamic> Prodotti)
    {
        decimal[] venditeMese = new decimal[12];
        string[] Mesi =
        {
        "Gennaio", "Febbraio", "Marzo", "Aprile", "Maggio", "Giugno", "Luglio", "Agosto", "Settembre", "Ottobre", "Novembre", "Dicembre"
    };

        foreach (var prodotto in Prodotti)
        {
            DateTime data = (DateTime)prodotto.Data;
            int meseIndex = data.Month - 1; // Indice mese corretto (0-11)
            venditeMese[meseIndex] += (decimal)prodotto.Importo;
        }

        // Creazione del dizionario per il grafico
        var datiGrafico = new Dictionary<string, decimal>();
        for (int i = 0; i < Mesi.Length; i++)
        {
            datiGrafico[Mesi[i]] = venditeMese[i];
        }

        DisegnaGraficoBarre("Vendite Mensili", datiGrafico);

        // Stampa del totale
        AnsiConsole.MarkupLine($"[red]TOTALE DEL MESE[/]: {venditeMese.Sum().ToString("C")}");
    }

    static void ReportVenditeProdotto(List<dynamic> Prodotti)
    {
        var venditePerProdotto = new Dictionary<string, decimal>();

        foreach (var prodotto in Prodotti)
        {
            string nomeProdotto = prodotto.Prodotto;
            decimal importo = prodotto.Importo;

            if (venditePerProdotto.ContainsKey(nomeProdotto))
            {
                venditePerProdotto[nomeProdotto] += importo;
            }
            else
            {
                venditePerProdotto[nomeProdotto] = importo;
            }
        }

        var tabella = new Table();
        tabella.AddColumn("Prodotto");
        tabella.AddColumn("Totale Vendite");

        foreach (var item in venditePerProdotto)
        {
            tabella.AddRow(item.Key, item.Value.ToString("C"));
        }

        AnsiConsole.Write(tabella);

        // Disegna il grafico a barre
        DisegnaGraficoBarre("Vendite per Prodotto", venditePerProdotto);
    }

    static void ReportVenditeFascePrezzo(List<dynamic> Prodotti)
    {
        // Definire le fasce di prezzo
        decimal[] limitiFasce = { 0, 10, 50, 100, 200, 500, 1000 };
        string[] descrizioniFasce = {
        "0 - 10 €",
        "10 - 50 €",
        "50 - 100 €",
        "100 - 200 €",
        "200 - 500 €",
        "500 - 1000 €",
        "Oltre 1000 €"
    };
        decimal[] venditePerFascia = new decimal[limitiFasce.Length + 1];

        // Calcolare il totale delle vendite per ciascuna fascia
        foreach (var prodotto in Prodotti)
        {
            decimal importo = prodotto.Importo;
            int indiceFascia = 0;

            while (indiceFascia < limitiFasce.Length && importo > limitiFasce[indiceFascia])
            {
                indiceFascia++;
            }

            venditePerFascia[indiceFascia] += importo;
        }

        // Visualizzare i risultati
        var tabella = new Table();
        tabella.AddColumn("Fascia di Prezzo");
        tabella.AddColumn("Totale Vendite");

        decimal maxVendite = venditePerFascia.Max();
        for (int i = 0; i < venditePerFascia.Length; i++)
        {
            string fascia = i < limitiFasce.Length ? descrizioniFasce[i] : "Oltre 1000 €";
            string totale = venditePerFascia[i].ToString("C");

            // Colorare la riga in base al totale delle vendite
            if (venditePerFascia[i] == maxVendite)
            {
                tabella.AddRow($"[bold green]{fascia}[/]", $"[bold green]{totale}[/]");
            }
            else
            {
                tabella.AddRow(fascia, totale);
            }
        }

        AnsiConsole.Write(tabella);

        // Disegna il grafico a barre
        var datiGrafico = new Dictionary<string, decimal>();
        for (int i = 0; i < venditePerFascia.Length; i++)
        {
            string fascia = i < limitiFasce.Length ? descrizioniFasce[i] : "Oltre 1000 €";
            datiGrafico[fascia] = venditePerFascia[i];
        }
        DisegnaGraficoBarre("Vendite per Fascia di Prezzo", datiGrafico);
    }

    static void RicercaProdotti(List<dynamic> prodotti)
    {
        // Chiedi all'utente di scegliere il criterio di ricerca
        var criterioRicerca = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Scegli il criterio di ricerca:")
                .AddChoices("Tutti i campi", "Importo", "Nome del prodotto", "Categoria")
        );

        // Chiedi all'utente di inserire il valore da cercare
        string valoreRicerca = AnsiConsole.Prompt(new TextPrompt<string>("Inserisci cosa vuoi cercare:")).ToLower();

        // Lista per i risultati della ricerca
        var risultatiRicerca = new List<dynamic>();

        // Filtra i prodotti basandosi sul criterio scelto
        foreach (var prodotto in prodotti)
        {
            bool trovato = false;

            switch (criterioRicerca)
            {
                case "Tutti i campi":
                    // Cerca in tutti i campi come stringa
                    trovato = prodotto.ToString().ToLower().Contains(valoreRicerca);
                    break;

                case "Importo":
                    // Controlla se il valore di ricerca è contenuto nel campo Importo
                    trovato = prodotto.Importo != null && prodotto.Importo.ToString().ToLower().Contains(valoreRicerca);
                    break;

                case "Nome del prodotto":
                    // Controlla se il valore di ricerca è contenuto nel campo Nome del prodotto
                    trovato = prodotto.Prodotto != null && prodotto.Prodotto.ToString().ToLower().Contains(valoreRicerca);
                    break;

                case "Categoria":
                    // Controlla se il valore di ricerca è contenuto nel campo Categoria
                    trovato = prodotto.Categoria != null && prodotto.Categoria.ToString().ToLower().Contains(valoreRicerca);
                    break;
            }

            if (trovato)
            {
                risultatiRicerca.Add(prodotto);
            }
        }

        // Mostra i risultati della ricerca o un messaggio se non ci sono risultati
        if (risultatiRicerca.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Nessun prodotto trovato.[/]");
        }
        else
        {
            VisualizzaProdottiInTabella(risultatiRicerca);
        }
    }



    // Funzioni di confronto per l'ordinamento
    static int ConfrontoNomeCrescente(dynamic x, dynamic y)
    {
        return string.Compare((string)x.Prodotto, (string)y.Prodotto);
    }

    static int ConfrontoNomeDecrescente(dynamic x, dynamic y)
    {
        return string.Compare((string)y.Prodotto, (string)x.Prodotto);
    }

    static int ConfrontoDataCrescente(dynamic x, dynamic y)
    {
        return DateTime.Compare((DateTime)x.Data, (DateTime)y.Data);
    }

    static int ConfrontoDataDecrescente(dynamic x, dynamic y)
    {
        return DateTime.Compare((DateTime)y.Data, (DateTime)x.Data);
    }

    static int ConfrontoCategoriaCrescente(dynamic x, dynamic y)
    {
        return string.Compare((string)x.Categoria, (string)y.Categoria);
    }

    static int ConfrontoCategoriaDecrescente(dynamic x, dynamic y)
    {
        return string.Compare((string)y.Categoria, (string)x.Categoria);
    }

    static int ConfrontoPrezzoCrescente(dynamic x, dynamic y)
    {
        return ((decimal)x.Importo).CompareTo((decimal)y.Importo);
    }

    static int ConfrontoPrezzoDecrescente(dynamic x, dynamic y)
    {
        return ((decimal)y.Importo).CompareTo((decimal)x.Importo);
    }
}
