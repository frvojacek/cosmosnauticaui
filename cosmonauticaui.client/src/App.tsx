import { useState, useEffect, FormEvent } from "react";
function App() {
    const [documents, setDocument] = useState([]);

    useEffect(() => {
        fetchDocuments()
    }, [])

    async function fetchDocuments() {
        const domain = import.meta.env.PROD ? "" : "https://localhost:7075";
        const response = await fetch(domain + "/api/Document")
        const documents = await response.json()
        setDocument(documents)
    }
    
    async function handleSubmit(event: FormEvent) {
        event.preventDefault();
        const target = event.target as HTMLFormElement;
        const domain = import.meta.env.PROD ? "" : "https://localhost:7075";
        await fetch(domain + "/api/Document", {
            method: "POST",
            body: new FormData(target)
        });
        fetchDocuments()
    }

    const listDocuments = documents.map((document, index) =>
        <tr key={index}>
            <td>{document}</td>
        </tr>
    )

    return (
        <>
            <form onSubmit={handleSubmit}>
                <label htmlFor="file">File</label>
                <input id="file" name="file" type="file" />
                <button>Submit</button>
            </form>
            <table>
                <tbody>
                    {listDocuments}
                </tbody>
            </table>
        </>
    );
}

export default App;