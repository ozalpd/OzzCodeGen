using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzzUtils
{
    public static partial class Text
    {
        public static List<string> GetLipsumWords(int wordCount)
        {
            var wordList = new List<string>();
            FillLipsums(wordCount, wordList);
            return wordList;
        }

        private static void FillLipsums(int wordCount, List<string> wordList)
        {
            foreach (var lipsum in _lipsum)
            {
                var words = lipsum.Split(' ');
                foreach (var item in words)
                {
                    wordList.Add(item);
                    if (wordList.Count >= wordCount)
                        break;
                }
                if (wordList.Count >= wordCount)
                    break;
            }
            if (wordList.Count < wordCount)
            {
                FillLipsums(wordCount, wordList);
            }
        }

        public static string CreateLipsumParagraphs(int paragraphCount)
        {
            int count = _lipsum.Count();
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 100; i++) { rnd.Next(0, count - 1); }

            for (int i = 0; i < paragraphCount; i++)
            {
                sb.AppendLine("<p>");
                int j = rnd.Next(0, count - 1);
                sb.Append(_lipsum[j].ToSentenceCase());
                sb.Append(".");
                if (_lipsum[j].Length < 300 && j < (_lipsum.Length - 1))
                {
                    sb.Append(_lipsum[j + 1].ToSentenceCase());
                    sb.Append(".");
                }
                sb.Append("</p>");
            }

            return sb.ToString();
        }

        public static string GetRandomLipsumSentence()
        {
            Random rnd = new Random();
            return LipsumSentences[rnd.Next(0, LipsumSentences.Length - 1)];
        }


        public static string[] LipsumSentences
        {
            get
            {
                if (_lipsumSentences == null || _lipsumSentences.Length == 0)
                {
                    var lipsums = new List<string>();
                    foreach (var s in _lipsum)
                    {
                        lipsums.Add(s.ToSentenceCase() + ".");
                    }
                    _lipsumSentences = lipsums.ToArray();
                }
                return _lipsumSentences;
            }
        }
        private static string[] _lipsumSentences;


        static string[] _lipsum =
            {
                "lorem ipsum dolor sit amet consetetur sadipscing elitr sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat sed diam voluptua at vero eos et accusam et justo duo dolores et ea rebum stet clita kasd gubergren no sea takimata sanctus est",
                "mi naskigxis en jorko anglujo je marto kiu estas lasesjarrego de la regxo karolo la unua infane mi sentadis grandandeziron por pasigi mian vivon sur la maro kaj pliagxante la deziroplifortigxis gxis fine mi forlasis mian lernejon kaj hejmon kajpiede mi trovis mian vojon al hull kie mi baldaux trovis okupadon sursxipo",
                "post kiam ni velveturis kelke da tagoj okazis ventego kaj kvinanoktela sxipo enfendigxis cxiuj al la pumpiloj rapidis la sxipon ni sentisgxemi en cxiuj siaj tabuloj kaj gxian trabajxon ektremi de la antauxa gxisla posta parto kaj baldaux klarigxis ke ne estas ia espero por gxi kajke cxio kion ni povas fari estas savi niajn vivojn",
                "unue ni pafadis pafilegojn por venigi helpon kaj post kelke datempo sxipo kusxante ne malproksime alsendis boaton por helpi nin sedla maro estis tro maltrankvila por gxi restadi sxipflanke tial nieljxetis sxnuregon kiun la boatanoj ekkaptis kaj firme fiksis kajtiamaniere ni cxiuj enboatigxis",
                "tamen vanigxis en tia maltrankvila maro por peni albordigxi la sxiponkiu alsendis la virojn aux aluzi la remilojn de la boato kaj ni nepovis ion fari krom gxin lasi peligxi teron",
                "duonhore nia sxipo trafis rifon kaj subakvigxis kaj gxin ni ne vidisplu tre malrapide ni alproksimigxis teron kiun iafoje ni vidis kiamajn la boato levigxis sur la supro de ia alta ondo kaj tie ni vidishomojn kurante amase tien kaj reen havante unu celon savi nin",
                "fine gxojege ni surterigxis kie bonsxance ni renkontis amikojn kiujdonis al ni helpon por reveturi al hull kaj se tiam mi havus labonan sencon por iri hejmon estus pli bone por mi",
                "la viro kies sxipo subakvigxis diris kun grava mieno junulo ne iruplu surmaron tiu ne estas la vivmaniero por vi kial do sinjorovi mem iros plu surmaron tiu estas alia afero mi estas elnutritapor la maro sed vi ne estas vi venis sur mian sxipon por eltrovi lastaton de vivo surmara kaj vi povas diveni tion kio okazos al vi sevi ne reiros hejmon dio ne benos vin kaj eble vi kauxzis tiun-cxitutan malbonon al ni",
                "mi ne parolis alian vorton al li kiun vojon li iris mi nek sciasnek deziris sciigxi cxar mi estis ofendita pro tiu-cxi malgxentilaparolado mi multe pensis cxu iri hejmon aux cxu iradi surmaron hontodetenis min pri iri hejmon kaj mi ne povis decidi la vivkuron kiunmi estis ironta",
                "kiel estis mia sorto travive cxiam elekti la plej malbonon tiel samemi nun faris mi havis oron en mia monujo kaj bonan vestajxon sur miakorpo sed surmaron mi ree iris",
                "sed nun mi havis pli malbonan sxancon ol iam cxar kiam ni estis tremalproksime enmaro kelke da turkoj en sxipeto plencxase alproksimigxisal ni ni levis tiom da veloj kiom niaj velstangoj povis elporti porke ni forkuru de ili tamen malgraux tio ni vidis ke niaj malamikojpli kaj pli alproksimigxis kaj certigxis ke baldaux ili atingos niansxipon",
                "fine ili atingis nin sed ni direktis niajn pafilegojn sur ilin kiokauxzis portempe ke ili deflanku sian vojon sed ili dauxrigis pafadonsur ni tiel longe kiel ili estis en pafspaco proksimigxante la duanfojon kelkaj viroj atingis la ferdekon de nia sxipo kaj ektrancxis lavelojn kaj ekfaris cxiuspecajn difektajxojn tial post kiam dek elniaj sxipanoj kusxas mortitaj kaj la plimulto el la ceteraj havasvundojn ni kapitulacis",
                "la cxefo de la turkoj prenis min kiel sian rabajxon al haveno okupitade mauxroj li ne agis al mi tiel malbone kiel mi lin unue jugxis sedli min laborigis kun la ceteraj de siaj sklavoj tio estis sxangxo enmia vivo kiun mi neniam antauxvidis ho ve kiom mia koro malgxojispensante pri tiuj kiujn mi lasis hejme al kiuj mi ne montris tiom dakomplezemo kiom diri adiauxi kiam mi iris surmaron aux sciigi tionkion mi intencas fari",
                "tamen cxio kion mi travivis tiam estas nur antauxgusto de la penadojkaj zorgoj kiujn de tiam estis mia sorto suferi",
                "unue mi pensis ke la turko kunprenos min kun si kiam li ree irossurmaron kaj ke mi iel povos liberigxi sed la espero nelonge dauxriscxar tiatempe li lasis min surtere por prizorgi liajn rikoltojntiamaniere mi vivis du jarojn tamen la turko konante kaj vidante minplu min pli kaj pli liberigis li unufoje aux dufoje cxiusemajneveturis en sia boato por kapti iajn platfisxojn kaj iafoje likunprenis min kaj knabon kun si cxar ni estas rapidaj cxe tia sportokaj tial li pli kaj pli sxatis min",
                "unu tagon la turko elsendis min viron kaj knabon boate por kaptikelke da fisxoj surmare okazas tia densa nebulo ke dekduhore ni nepovas vidi la teron kvankam ni ne estas pli ol duonmejlon 00metrojn de la terbordo kaj morgauxtage kiam la suno levigxis niaboato estas enmaro almenaux dek mejlojn kilometrojn de laterbordo la vento vigle blovis kaj ni cxiuj tre bezonis nutrajxon sedfine per la helpo de remiloj kaj veloj ni sendangxere reatingis laterbordon",
                "kiam la turko sciigxis kiamaniere ni vojperdis li diris ke de nunkiam li velveturos li prenos boaton kiu enhavos cxion kion nibezonus se ni longatempe estus detenataj surmare tial li farigisgrandan kajuton en la longboato de sia sxipo kiel ankaux cxambron por nisklavoj unu tagon li min sendis por ke mi ordigu la boaton pro tioke li havas du amikojn kiuj intencas veturi kun li por fisxkapti sedkiam la tempo alvenis ili ne veturas tial li sendis min viron kajknabon -- kies nomo estas zuro -- por kapti kelke da fisxoj por lagastoj kiuj estas vespermangxontaj kun li",
                "subite eniris en mian kapon la ideo ke nun estas bona okazo boateforkuri kaj liberigxi tial mi tuj prenis tiom da nutrajxo kiom mipovas havigi kaj mi diris al la viro ke estus tro malrespektemangxante la panon metitan en la boaton por la turko li diris ke lipensas tiel same tial li alportis sakon da rizo kaj kelke da ruskojkukoj",
                "dum la viro estis surtere mi provizis iom da vino pecegon da vaksosegilon hakilon fosilon iom da sxnurego kaj cxiuspecajn objektojnkiuj eble estos utilaj al ni mi sciis kie trovigxas la vinkesto de laturko kaj mi gxin metis surboaton dum la viro estas surtere per aliaruzo mi havigis cxion kion mi bezonis mi diris al la knabo lapafiloj de la turko estas en la boato sed ne trovigxas ia pafajxo cxuvi pensas ke vi povas havigi iom da gxi vi scias kie gxi estaskonservata kaj eble ni volos pafi birdon aux du li do alportis kestokaj saketon kiuj enhavas cxion kion ni eble bezonas por la pafilojtiujn-cxi mi metis surboaton kaj poste velveturis por fisxkapti",
                "la vento blovis de la nordo aux nordokcidento tia vento estis malbonapor mi cxar se gxi estus de la sudo mi estus povinta velveturi al laterbordo de hispanujo tamen de kiu ajn loko la vento blovos miestis decidinta forkuri kaj lasi la ceterajn al ilia sorto mi domallevis miajn hokfadenojn kvazaux fisxkapti sed mi zorgis ke mi havumalbonan sukceson kaj kiam la fisxoj mordis mi ilin ne eltiris cxarmi deziris ke la mauxro ilin ne vidu mi diris al li tiu-cxi lokoestas nebona ni ne kaptos fisxojn tie-cxi ni devas iom antauxen irinu la mauxro pensis ke tion fari ne estos malbone li levis lavelojn kaj cxar la direktilo estis en miaj manoj mi elsendis laboaton unu mejlon aux plu enmaron kaj poste gxin haltigis kvazaux miintencas fisxkapti",
                "nun mi pripensis tiu-cxi estas mia okazo liberigxi tial mi transdonisla direktilon al la knabo kaj tiam ekprenis la mauxron cxirkaux latalio kaj eljxetis lin el la boato",
                "malsupren li falis sed baldaux reaperis por ke li povis nagxi kvazauxanaso li diris ke li volonte irus cxirkaux la mondo kun mi se mienprenus lin",
                "iom timante ke li surrampos la boatflankon kaj reenigxos perforte midirektis mian pafilon sur lin kaj diris vi facile povas nagxialteron se vi tion deziras tial rapidigxu tien plie se vi reenalproksimigxos la boaton vi ricevos kuglon tra la kapo cxar mi de nunintencas esti libera viro",
                "tiam li eknagxis kaj sendube sendangxere atingis la terbordon cxar lamaro estis tre trankvila",
                "unue mi intencis kunpreni la mauxron kun mi kaj nagxigi zuron alteronsed la mauxro ne estis viro pri kiu mi povis konfidi",
                "post kiam li forigxis mi diris al zuro se vi jxuros ke vi estosfidela al mi vi iam farigxos grava viro se vi ne jxuros mi certeankaux vin eljxetos el la boato",
                "la knabo tiel dolcxe ridetis kiam li jxuris resti fidela al mi ke milin ne povis dubi en mia koro",
                "dum ankoraux ni povis vidi la mauxron survoje alteren ni antauxen irisenmaron por ke li kaj tiuj kiuj nin vidis de la terbordo kredu keni iros al la influejo de la markolo cxar neniu velveturis al la sudamarbordo cxar tie logxas gento da homoj kiuj laux sciigoj mortigas kajmangxas siajn malamikojn",
                "tiam mi direktis mian veturadon oriente por ke ni lauxlongiru lamarbordon kaj havante favoron venton kaj trankvilan maron nimorgauxtagmeze estis malapud kaj preter la povo de la turko",
                "ankoraux mi timis ke mi estus kaptota de la mauxroj tial mi ne volisiri surteron tage duonlume ni direktis nian boaton alteren kajatingis la enfluejon riveretan de kiu mi pensis ni povos nagxisurteron kaj tiam rigardi la cxirkauxajxojn sed kiam malheligxis lalumo ni auxdis strangajn sonojn bojojn kriegojn gruntojnblekadojn la malfelicxa knabo diris ke li ne kuragxas iri surteronantaux la tagigxo nu mi diris tiuokaze ni atendu sed tagepovas vidi nin la homoj kiuj eble nin pli malhelpos ol sovagxajbestoj tiam ni pafilos ilin ridante diris zuro kaj forkuriguilin",
                "mi gxojis vidi ke la knabo montras tiom da gajeco kaj mi donis al liiom da pano kaj rizo tiunokte ni silente ripozis sed ne longedormis cxar post kelke da horoj iaj grandegaj bestoj malsuprenvenisal la maro por sin bani la malfelicxa knabo ektremis de kapo alpiedoj pro la vidajxo unu el tiuj bestoj alproksimigxis nian boatonkaj kvankam estis tro mallume por gxin bone vidi ni auxdis gxin blovikaj sciis pro gxia bruego ke gxi certe estas granda fine la brutotiom alproksimigxis la boaton kiom la longeco de du remiloj tial mipafis sur gxin kaj gxi nagxis alteren",
                "la blekegoj kaj kriegoj kiujn faris bestoj kaj birdoj pro la bruo demia pafilo sxajne montris ke ni faris malbonan elekton por surterejosed vole ne vole ni devis iri surtere por sercxi fresxan fonton porke ni povu plenigi niajn barelojn zuro diris ke li eltrovus cxu lafontaj akvoj tauxgas por trinki se mi permesus al li preni unu el labotelegoj kaj ke li gxin reportos plenigitan se la akvo estas bonakial vi volas iri mi diris kial mi ne estas ironta vi povasresti en la boato kontrauxe zuro diris se la sovagxuloj venos ilimin mangxu sed vi forkuru mi devis ami la junulon pro la afablaparolado nu mi diris ni ambaux iros kaj se la sovagxuloj venosni ilin mortigu ja ili ne mangxos aux vin aux min",
                "mi donis al zuro iom da rumo el la kesto de la turko por reforti linkaj ni iris surteron la knabo ekiris kun sia pafilo mejlon de laloko kie ni surteriris kaj li revenis kun leporo kiun li mortpafiskaj kiun ni gxoje kuiris kaj mangxis laux la bona novajxo kiun liraportis li eltrovis fonton kaj ne vidis sovagxulojn",
                "mi divenis ke la promontoro de la verdaj insuloj ne estasmalproksime cxar mi vidis la supron de la granda pinto kiun kiel misciis estas apud ili mia sola espero estis ke lauxlongirante laterbordon ni trovos sxipon kiu ensxipigos nin kaj tiam kaj ne antauxtiam mi sentos kvazaux libera viro unuvorte mi konfidis mian sortonal la sxanco aux renkonti ian sxipon aux morti",
                "surteron ni ekvidis iujn homojn kiuj staras kaj rigardas nin iliestis nigraj kaj ne portis vestajxon mi estus irinta surteron al ilised zuro -- kiu sciis plej bone -- diris ne vi iru ne vi iru tialmi direktis la boaton lauxteron por ke mi povu paroli kun ili kaj ililongaspace iradis laux ni mi ekvidis ke unu havas lancon en mano",
                "mi faris signojn ke ili alportu iom da nutrajxo al mi kaj ilisiaparte faris signojn ke mi haltu mian boaton tial mi demetis lasupran parton de mia velo kaj haltis tiam du el ili ekforkuris kajduonhore revenis kun iom da sekigxita viando kaj ia greno kiu kreskasen tiu parto de la mondo tion-cxi ni deziregis sed ne sciis kielhavigi gxin cxar ni ne kuragxis iri surteron al ili nek ili kuragxisalproksimigxi al ni",
                "fine ili eltrovis peron sendangxeran por ni cxiuj alportante lanutrajxon al la marbordo ili gxin demetis kaj tre fortirigis si mem dumni gxin prenis ni faris signojn por montri nian dankon ne havante ionalian kion ni povas doni al ili sed bonsxance ni baldaux kaptisgrandan donacon por ili cxar du sovagxaj bestoj de la sama speco prikiu mi jam priparolis venis plencxase de la montetoj al la maro",
                "ili nagxis kvazaux ili venis por sportigi cxiuj forkuris de ili kromtiu kiu portas la lancon unu el tiuj bestoj alproksimigxis nianboaton tial mi gxin atendis kun mia pafilo kaj tuj kiam gxi estis enpafspaco mi gxin pafis tra la kapo dufoje gxi subakvigxis kaj dufoje gxisuprenlevigxis kaj poste gxi nagxis alteren kaj falis senviva la virojtiom timis pro la pafilbruo kiom ili antauxe timis je la vidajxo de labestoj sed kiam mi faris signojn por ke ili venu al la marbordo ilituj venis",
                "ili rapidis al sia rabajxo kaj tordante cxirkaux gxi sxnuregon ili gxinsendangxere eltiris surteron",
                "ni nun lasis niajn sovagxulojn kaj iradis dekdu tagojn plu la terbordoantaux ni etendis sin kvar aux kvin mejlojn aux kilometrojnbekforme kaj ni devis veturi iom de la terbordo por atingi tiunterpinton tiel ke ni portempe ne vidis teron",
                "mi konfidis la direktilon al zuro kaj sidigxis por pripensi tion kionestos plej bone nun fari kiam subite mi auxdis ke la knabo kriassxipon kun velo sxipon kun velo li ne montris multe da gxojo je lavidajxo opiniante ke la sxipo venis por repreni lin sed mi bonescias laux la sxajno ke gxi ne estas iu el la sxipoj de la turko",
                "mi levis kiel eble plej multe da veloj por renkonti la sxipon gxiavojekaj ordonis al zuro ke li ekpafu pafilon cxar mi esperis ke se tiujkiuj estas sur la ferdeko ne povus auxdi la sonon ili vidus lafumigadon ili ja gxin vidis kaj tuj demetis siajn velojn por ke nipovu atingi ilin kaj trihore ni estis cxe la sxipflanko la virojparolis kun ni per la franca lingvo sed ni ne povis kompreni tionkion ili diras fine skoto sursxipe diris per mia lingvo kiu viestas de kien vi venas mi diris al li iomvorte kiel mi liberigxisde la mauxroj",
                "tiam la sxipestro invitis min veni sxipbordon kaj ensxipis min zuronkaj cxiujn miajn posedajxojn mi diris al li ke li havu cxion kion mihavas sed li respondis vi estas rericevonta viajn posedajxojn postkiam ni atingos teron cxar mi por vi nur faris tion kion por mi vifarus samstate",
                "li pagis al mi multan monon por mia boato kaj diris ke mi ricevosegalan monon por zuro se mi lin fordonus sed mi diris al li keliberigxinte kun helpo de la knabo mi lin ne volas vendi li diris keestas juste kaj prave por mi tiel senti sed se mi decidus fordonizuron li estus liberigota dujare tial cxar la sklavo deziris iri minenial diris ne trisemajne mi alvenis al cxiuj sanktuloj golfeto kajnun mi estis liberulo",
                "mi ricevis multan monon por cxiujn miaj posedajxoj kaj kun gxi mi irissurteron sed mi tute ne sciis kion nun fari fine mi renkontisviron kies stato estas laux la mia kaj ni ambaux akiris pecon da teropor gxin prilabori mia farmilaro laux la lia estis malgranda sed niproduktigis la farmojn suficxe por subteni nin sed ne plu ni bezonishelpon kaj nun mi eksentis ke mi eraris ellasante la knabon",
                "mi tute ne sxatis tiun manieron de vivo kion mi pensis cxu mi venistian longan vojon por fari tion kion mi lauxbone povus fari hejme kajkun miaj parencoj cxirkaux mi kaj pligrandigxis mia malgxojo cxar labonamiko kiu min alsxipis tien-cxi intencas nune lasi tiun-cxiterbordon",
                "kiam mi estis knabo kaj ekiris surmaron mi metis en la manojn de miaonklino iom da mono pri kiu mia bonamiko diris ke mi bone farus semi gxin elspezus pro mia bieno tial post kiam li revenis hejmon lialsendis iom da gxi kontante kaj la restajxon kiel tukoj sxtofojlanajxoj kaj similajxoj kiujn li acxetis mia onklino tiam metis enliajn manojn iom da livroj kiel donaco al li por montri siandankecon pro cxio kion li faris por mi kaj per tiu mono li afableacxetis sklavon por mi intertempe mi jam acxetis sklavon tial mi nunhavas du kaj cxio prosperis dum la sekvanta jaro",
                "sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo",
                "nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt",
                "neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem",
                "ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
                "at vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga",
                "et harum quidem rerum facilis est et expedita distinctio",
                "nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus",
                "temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae",
                "itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat"
            };
    }
}
